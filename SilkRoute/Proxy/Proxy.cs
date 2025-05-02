using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SilkRoute.Interfaces;

namespace SilkRoute.Proxy
{
    /// <summary>
    /// DispatchProxy implementation that handles method calls on the generated proxy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Proxy<T> : DispatchProxy where T : IMicroserviceClient
    {
        private HttpClient? _httpClient;

        internal void SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (_httpClient == null)
                throw new InvalidOperationException("HttpClient not initialized.");
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));

            var httpAttr = targetMethod.GetCustomAttributes().OfType<HttpMethodAttribute>().FirstOrDefault();
            var routeAttr = targetMethod.GetCustomAttributes().OfType<RouteAttribute>().FirstOrDefault();
            if (httpAttr == null && routeAttr == null)
                throw new InvalidOperationException("Method must have an HttpMethod or Route attribute.");

            var httpMethod = httpAttr?.HttpMethods.FirstOrDefault() ?? HttpMethods.Get;
            var template = httpAttr?.Template ?? routeAttr?.Template;
            if (string.IsNullOrEmpty(template))
                throw new InvalidOperationException("Route template is not specified.");

            string requestUri = template;
            HttpContent? content = null;
            var parameters = targetMethod.GetParameters();
            if (args != null && args.Length > 0 && parameters.Length > 0)
            {
                if (HttpMethods.IsGet(httpMethod))
                {
                    var queryDict = parameters
                        .Select((p, i) => new { p.Name, Value = i < args.Length ? args[i] : null })
                        .Where(x => x.Value != null)
                        .ToDictionary(x => x.Name!, x => x.Value!.ToString()!);
                    requestUri = QueryHelpers.AddQueryString(requestUri, queryDict);
                }
                else
                {
                    var bodyDict = parameters
                        .Select((p, i) => new { p.Name, Value = i < args.Length ? args[i] : null })
                        .Where(x => x.Value != null)
                        .ToDictionary(x => x.Name!, x => x.Value);
                    var json = JsonConvert.SerializeObject(bodyDict);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
            }

            var request = new HttpRequestMessage(new HttpMethod(httpMethod), requestUri)
            {
                Content = content
            };
            var response = _httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(responseContent);

            var returnType = targetMethod.ReturnType;
            bool isTask = typeof(Task).IsAssignableFrom(returnType);
            Type resultType;

            if (isTask && returnType.IsGenericType)
                resultType = returnType.GetGenericArguments()[0];
            else if (!isTask && returnType.IsGenericType)
                resultType = returnType.GetGenericArguments()[0];
            else if (isTask)
                resultType = typeof(void);
            else
                resultType = returnType;

            object? deserialized = null;
            if (resultType != typeof(void))
                deserialized = JsonConvert.DeserializeObject(responseContent, resultType)
                               ?? throw new InvalidOperationException("Failed to deserialize response.");

            if (isTask)
            {
                if (!returnType.IsGenericType)
                    return Task.CompletedTask;

                var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
                dynamic tcs = Activator.CreateInstance(tcsType)!;
                tcs.SetResult((dynamic)deserialized!);
                return tcs.Task;
            }
            else
            {
                if (returnType.IsGenericType)
                    return Activator.CreateInstance(returnType, deserialized)!;
                return deserialized!;
            }
        }
    }
}
