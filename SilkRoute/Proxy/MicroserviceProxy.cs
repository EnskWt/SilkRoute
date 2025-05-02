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
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
    public class MicroserviceProxy<T> : DispatchProxy where T : IMicroserviceClient
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

            var method = httpAttr?.HttpMethods.First() ?? HttpMethods.Get;
            var template = httpAttr?.Template ?? routeAttr?.Template;
            if (string.IsNullOrEmpty(template))
                throw new InvalidOperationException("Route template is not specified.");

            var parameters = targetMethod.GetParameters();
            var uri = template!;
            HttpContent? content = null;

            if (args?.Length > 0 && parameters.Length > 0)
            {
                var entries = parameters
                    .Select((p, i) => new { p.Name, Value = i < args.Length ? args[i] : null })
                    .Where(x => x.Value != null)
                    .ToList();

                if (HttpMethods.IsGet(method))
                {
                    var dict = entries.ToDictionary(x => x.Name!, x => x.Value!.ToString()!);
                    uri = QueryHelpers.AddQueryString(uri, dict);
                }
                else
                {
                    var dict = entries.ToDictionary(x => x.Name!, x => x.Value);
                    content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                }
            }

            var response = _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(method), uri) { Content = content }).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(json);

            var returnType = targetMethod.ReturnType;
            bool isAsync = typeof(Task).IsAssignableFrom(returnType);

            Type responseType = isAsync
                ? (returnType.IsGenericType
                    ? returnType.GetGenericArguments()[0]
                    : typeof(void))
                : returnType;

            Type payloadType = responseType;
            if (responseType.IsGenericType
                && typeof(IConvertToActionResult).IsAssignableFrom(responseType))
            {
                payloadType = responseType.GetGenericArguments()[0];
            }

            object? payload = payloadType == typeof(void)
                ? null
                : JsonConvert.DeserializeObject(json, payloadType);

            object result = payload!;
            if (responseType != payloadType)
            {
                result = Activator.CreateInstance(responseType, payload)!;
            }

            if (isAsync)
            {
                if (responseType == typeof(void))
                    return Task.CompletedTask;

                var fromResult = typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(responseType);
                return (Task)fromResult.Invoke(null, new[] { result })!;
            }

            return result;
        }
    }
}
