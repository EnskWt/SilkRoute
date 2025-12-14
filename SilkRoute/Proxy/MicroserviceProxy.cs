using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SilkRoute.Interfaces;
using SilkRoute.Tools.ActionResultTools;
using SilkRoute.Tools.RequestTools;
using SilkRoute.Tools.ResponseTools;

namespace SilkRoute.Proxy
{
    /// <summary>
    /// IAsyncInterceptor implementation that handles method calls on the generated proxy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MicroserviceProxy<T> : IAsyncInterceptor
        where T : IMicroserviceClient
    {
        private readonly HttpClient _httpClient;

        public MicroserviceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            var method = invocation.Method;
            var returnType = method.ReturnType;

            var result = Invoke(method, invocation.Arguments)
                .GetAwaiter()
                .GetResult();

            if (returnType != typeof(void))
            {
                invocation.ReturnValue = result;
            }
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsyncNonGeneric(invocation);
        }

        private async Task InterceptAsyncNonGeneric(IInvocation invocation)
        {
            var method = invocation.Method;
            await Invoke(method, invocation.Arguments).ConfigureAwait(false);
        }


        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsyncGeneric<TResult>(invocation);
        }

        private async Task<TResult> InterceptAsyncGeneric<TResult>(IInvocation invocation)
        {
            var method = invocation.Method;

            var result = await Invoke(method, invocation.Arguments)
                .ConfigureAwait(false);

            return (TResult)result!;
        }

        private async Task<object?> Invoke(MethodInfo targetMethod, object?[] args)
        {
            EnsureInitialized(targetMethod);

            var (method, uri, content, headers) = PrepareRequest(targetMethod, args);

            var response = await SendRequest(method, uri, content, headers)
                .ConfigureAwait(false);

            var (responseType, _) = GetReturnTypeInfo(targetMethod);
            var (isActionResult, payloadType) = GetPayloadInfo(responseType);

            object? payload = await ReadResponse(response, responseType, payloadType, isActionResult)
                .ConfigureAwait(false);

            object result = BuildResult(response, responseType, isActionResult, payload);

            return result;
        }


        /// <summary>
        /// Ensures that the proxy is initialized with a valid HttpClient and method.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void EnsureInitialized(MethodInfo? targetMethod)
        {
            if (_httpClient == null)
                throw new InvalidOperationException("HttpClient not initialized.");
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));

            var hasHttpAttr = targetMethod.GetCustomAttributes()
                .OfType<HttpMethodAttribute>().Any();
            var hasRouteAttr = targetMethod.GetCustomAttributes()
                .OfType<RouteAttribute>().Any();
            if (!hasHttpAttr && !hasRouteAttr)
                throw new InvalidOperationException("Method must have an HttpMethod or Route attribute.");
        }

        /// <summary>
        /// Prepares the request by extracting the HTTP method, URI, and content from the target method.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private (string method, string uri, HttpContent? content, IDictionary<string, string> headers) PrepareRequest(
            MethodInfo targetMethod,
            object?[]? args)
        {
            var httpAttr = targetMethod.GetCustomAttributes().OfType<HttpMethodAttribute>().FirstOrDefault();
            var routeAttr = targetMethod.GetCustomAttributes().OfType<RouteAttribute>().FirstOrDefault();
            var method = httpAttr?.HttpMethods.First() ?? HttpMethods.Get;
            var template = httpAttr?.Template ?? routeAttr?.Template
                           ?? throw new InvalidOperationException("Route template is not specified.");

            var uri = template;

            var parameters = targetMethod.GetParameters();

            var requestBuilder = new RequestBuilder(new HttpMethod(method), uri);

            requestBuilder.BindAllParameters(parameters, args);

            requestBuilder.EnsureNoBodyAndFormDataConflict();

            return requestBuilder.BuildRequest();
        }

        /// <summary>
        /// Sends the HTTP request and returns response.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<HttpResponseMessage> SendRequest(
            string method,
            string uri,
            HttpContent? content,
            IDictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), uri)
            {
                Content = content
            };

            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return await _httpClient!.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Properly reads response content
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseType"></param>
        /// <param name="payloadType"></param>
        /// <returns></returns>
        private Task<object?> ReadResponse(
            HttpResponseMessage response,
            Type responseType,
            Type payloadType,
            bool isActionResult)
        {
            var responseReader = new ResponseReader();

            return responseReader.ReadResponseContent(
                response,
                responseType,
                payloadType,
                isActionResult);
        }

        /// <summary>
        /// Gets the return type information of the target method.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <returns></returns>
        private (Type responseType, bool isAsync) GetReturnTypeInfo(MethodInfo targetMethod)
        {
            // If the method return type is Task, we need to get the generic argument type or void
            var returnType = targetMethod.ReturnType;
            bool isAsync = typeof(Task).IsAssignableFrom(returnType);
            var responseType = isAsync
                ? (returnType.IsGenericType ? returnType.GetGenericArguments()[0] : typeof(void))
                : returnType;

            return (responseType, isAsync);
        }

        /// <summary>
        /// Gets the payload information from the response type.
        /// </summary>
        /// <param name="responseType"></param>
        /// <returns></returns>
        private (bool isActionResult, Type payloadType) GetPayloadInfo(Type responseType)
        {
            // Check if the response type is an ActionResult or a type that can be converted to ActionResult
            bool isActionResult = typeof(IActionResult).IsAssignableFrom(responseType)
                || typeof(IConvertToActionResult).IsAssignableFrom(responseType);

            // If the response type is an ActionResult, we need to get the payload type if it's specified in the generic type or default to string
            var payloadType = isActionResult
                ? (responseType.IsGenericType
                    ? responseType.GetGenericArguments()[0]
                    : responseType)
                : responseType;

            return (isActionResult, payloadType);
        }

        /// <summary>
        /// Builds the result based on the response and payload.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseType"></param>
        /// <param name="isActionResult"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private object BuildResult(
            HttpResponseMessage response,
            Type responseType,
            bool isActionResult,
            object? payload)
        {
            if (!isActionResult)
                return payload!;

            var actionResultWrapperFactory = new ActionResultWrapperFactory();

            return actionResultWrapperFactory.Wrap(response, responseType, payload);
        }
    }
}
