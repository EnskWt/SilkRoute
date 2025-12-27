using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SilkRoute.Abstractions;
using SilkRoute.Tools.ActionResultTools;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorFactory;
using SilkRoute.Tools.RequestTools;
using SilkRoute.Tools.ResponseTools;

namespace SilkRoute.Proxy;

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
        EnsureValidInvocation(targetMethod);

        var (method, uri, content, headers) = PrepareRequest(targetMethod, args);

        var response = await SendRequest(method, uri, content, headers)
            .ConfigureAwait(false);

        var actionReturnDescriptor = GetActionReturnDescriptor(targetMethod);

        var actionReturnValue = await ReadResponse(response, actionReturnDescriptor)
            .ConfigureAwait(false);

        var result = BuildActionResultIfNeeded(response, actionReturnDescriptor, actionReturnValue);

        return result;
    }
    
    private void EnsureValidInvocation(MethodInfo? targetMethod)
    {
        if (_httpClient == null)
        {
            throw new InvalidOperationException("HttpClient not initialized.");
        }

        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        var hasHttpAttr = targetMethod.GetCustomAttributes()
            .OfType<HttpMethodAttribute>().Any();
        var hasRouteAttr = targetMethod.GetCustomAttributes()
            .OfType<RouteAttribute>().Any();

        if (!hasHttpAttr && !hasRouteAttr)
        {
            throw new InvalidOperationException("Method must have an HttpMethod or Route attribute.");
        }
    }

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
    
    private static IActionReturnDescriptor GetActionReturnDescriptor(MethodInfo targetMethod)
    {
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        var returnType = targetMethod.ReturnType;

        var returnsTask = typeof(Task).IsAssignableFrom(returnType);

        var resultType = returnsTask
            ? (returnType.IsGenericType ? returnType.GetGenericArguments()[0] : typeof(void))
            : returnType;

        return ActionReturnDescriptorFactory.Create(resultType);
    }
    
    private Task<object?> ReadResponse(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor)
    {
        var responseReader = new ResponseReader(response, actionReturnDescriptor);
        return responseReader.ReadResponseContent();
    }
    
    private object BuildActionResultIfNeeded(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object? actionReturnValue)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();

        if (!actionReturnType.IsActionResultLikeType())
        {
            return actionReturnValue!;
        }

        var actionResultWrapperFactory =
            new ActionResultWrapperFactory(response, actionReturnDescriptor, actionReturnValue);

        return actionResultWrapperFactory.Wrap();
    }
}