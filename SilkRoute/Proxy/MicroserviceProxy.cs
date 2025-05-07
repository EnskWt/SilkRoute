﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SilkRoute.Interfaces;
using SilkRoute.Tools.RequestTools;

namespace SilkRoute.Proxy
{
    /// <summary>
    /// DispatchProxy implementation that handles method calls on the generated proxy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MicroserviceProxy<T> : DispatchProxy where T : IMicroserviceClient
    {
        // HttpClient to be used for making requests
        private HttpClient? _httpClient;

        // Method to set to created HttpClient instance
        internal void SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Invokes the method on the proxy.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            EnsureInitialized(targetMethod);

            var (method, uri, content, headers) = PrepareRequest(targetMethod!, args);
            var (response, json) = SendAndReadResponse(method, uri, content, headers);

            var (responseType, isAsync) = GetReturnTypeInfo(targetMethod!);
            var (isActionResult, payloadType) = GetPayloadInfo(responseType);

            object? payload = DeserializePayload(json, payloadType);
            object result = BuildResult(response, responseType, isActionResult, payload);

            return WrapIfAsync(result, responseType, isAsync);
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
        /// Sends the HTTP request and reads the response.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private (HttpResponseMessage response, string json) SendAndReadResponse(string method, string uri, HttpContent? content, IDictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), uri) { Content = content };

            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = _httpClient!.SendAsync(request).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(json);

            return (response, json);
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
                    : typeof(string))
                : responseType;

            return (isActionResult, payloadType);
        }

        /// <summary>
        /// Deserializes the JSON payload into the specified type.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="payloadType"></param>
        /// <returns></returns>
        private object? DeserializePayload(string json, Type payloadType)
        {
            // If the payload type is void, return null
            if (payloadType == typeof(void))
                return null;

            // If the payload type is a string, return the JSON string
            if (payloadType == typeof(string))
                return json;

            // If the payload type is a object, deserialize it into a specified payload type
            return JsonConvert.DeserializeObject(json, payloadType)!;
        }

        /// <summary>
        /// Builds the result based on the response and payload.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseType"></param>
        /// <param name="isActionResult"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private object BuildResult(HttpResponseMessage response, Type responseType, bool isActionResult, object? payload)
        {
            // If the response type is not an ActionResult, we need to return the payload directly, otgerwise we need to wrap it in an ActionResult
            if (!isActionResult)
                return payload!;

            // Receive the status code from the original response
            var statusCode = (int)response.StatusCode;

            // For ActionResult<>:

            // If the response type is an ActionResult with a generic type, we need to create an instance of the ActionResult with the payload
            if (responseType.IsGenericType)
            {
                var argType = responseType.GetGenericArguments()[0];

                object? effectivePayload = payload
                  ?? (argType.IsValueType
                        ? Activator.CreateInstance(argType)!
                        : null);

                var ctor = responseType
                    .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(ci =>
                    {
                        var ps = ci.GetParameters();
                        return ps.Length == 1
                               && ps[0].ParameterType == argType;
                    });

                if (ctor == null)
                {
                    throw new InvalidOperationException(
                        $"No suitable constructor found for {responseType.Name}({argType.Name})");
                }

                return ctor.Invoke(new[] { effectivePayload! });
            }

            // For non-generic ActionResult that inherits from ObjectResult:

            // If the response type is not direct ActionResult with generic type, we need to create an instance of the specific ActionResult type with payload as value (usually string)
            if (typeof(ObjectResult).IsAssignableFrom(responseType))
            {
                var inst = Activator.CreateInstance(responseType)!;
                var obj = (ObjectResult)inst;
                obj.Value = payload;
                obj.StatusCode = statusCode;
                return inst;
            }

            // For all other cases, we need to create an instance of the ContentResult with payload as value (usually string) or StatusCodeResult:

            // If the payload is null, we need to create an instance of the StatusCodeResult with the status code
            if (payload == null)
            {
                return Activator.CreateInstance(typeof(StatusCodeResult), statusCode)!;
            }

            // For cases when payload is not null and return type is like IActionResult, ActionResult etc., we need to return result as ContentResult as we received since we can't create an instance of IActionResult or ActionResult and we can't know real return type (like OkResult, JsonResult etc.) that was returned in method as well.
            return new ContentResult
            {
                Content = payload.ToString()!,
                ContentType = "application/json",
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Wraps the result in a Task if the method is asynchronous.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="responseType"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        private object? WrapIfAsync(object result, Type responseType, bool isAsync)
        {
            // If the method is not asynchronous, return the result directly
            if (!isAsync)
                return result;

            // If the response type is void (in other words Task without generic type), return a completed Task
            if (responseType == typeof(void))
                return Task.CompletedTask;

            // If the response type is a Task with a generic type, we need to wrap the result in a Task
            var fromResult = typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(responseType);
            return (Task)fromResult.Invoke(null, new[] { result })!;
        }
    }
}
