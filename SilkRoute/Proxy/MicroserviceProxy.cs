using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using SilkRoute.Helpers;
using SilkRoute.Interfaces;

namespace SilkRoute.Proxy
{
    /// <summary>
    /// DispatchProxy implementation that handles method calls on the generated proxy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MicroserviceProxy<T> : DispatchProxy where T : IMicroserviceClient
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
            HttpContent? content = null;

            var parameters = targetMethod.GetParameters();
            var routeParams = new Dictionary<string, string>();
            var headerParams = new Dictionary<string, string>();
            (string Name, object Value)? explicitBody = null;
            var defaultParams = new List<(string Name, object Value)>();

            var placeholders = RequestHelper.GetPlaceholders(template);
            var queryBuilder = new QueryBuilder();

            MultipartFormDataContent? formContent = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                var val = args != null && i < args.Length ? args[i] : null;
                if (val == null) continue;

                var name = p.Name!;
                var hasFromRoute = p.GetCustomAttribute<FromRouteAttribute>() != null;
                var mappedType = RequestHelper.MapTypeName(p.ParameterType);
                var isPlaceholder = placeholders.Any(pi =>
                    pi.Name == name && (pi.Type == null || (mappedType != null && pi.Type == mappedType)));


                if (hasFromRoute || isPlaceholder)
                {
                    routeParams[name] = val.ToString()!;
                    continue;
                }

                if (p.GetCustomAttribute<FromFormAttribute>() != null)
                {
                    formContent ??= new MultipartFormDataContent();
                    if (val is IFormFile formFile)
                    {
                        var stream = formFile.OpenReadStream();
                        var streamContent = new StreamContent(stream);
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = $"\"{name}\"",
                            FileName = $"\"{formFile.FileName}\""
                        };
                        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(formFile.ContentType);
                        formContent.Add(streamContent, name, formFile.FileName);
                    }
                    else if (val is IEnumerable<IFormFile> formFiles)
                    {
                        foreach (var file in formFiles)
                        {
                            var stream = file.OpenReadStream();
                            var streamContent = new StreamContent(stream);
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = $"\"{name}\"",
                                FileName = $"\"{file.FileName}\""
                            };
                            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                            formContent.Add(streamContent, name, file.FileName);
                        }
                    }
                    else if (RequestHelper.IsPrimitive(val.GetType()) || val is string)
                    {
                        formContent.Add(new StringContent(val.ToString()!), name);
                    }
                    else if (val is Stream s)
                    {
                        var sc = new StreamContent(s);
                        sc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = $"\"{name}\"",
                            FileName = "\"file\""
                        };
                        formContent.Add(sc, name, "file");
                    }
                    else if (val is byte[] bytes)
                    {
                        var bc = new ByteArrayContent(bytes);
                        bc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = $"\"{name}\"",
                            FileName = "\"file\""
                        };
                        formContent.Add(bc, name, "file");
                    }
                    else if (val is HttpContent hc)
                    {
                        formContent.Add(hc, name);
                    }
                    else
                    {
                        foreach (var prop in val.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            var propVal = prop.GetValue(val);
                            if (propVal == null) continue;
                            var propName = prop.Name;
                            if (RequestHelper.IsPrimitive(prop.PropertyType) || propVal is string)
                            {
                                formContent.Add(new StringContent(propVal.ToString()!), propName);
                            }
                            else if (propVal is IEnumerable<object> coll)
                            {
                                foreach (var item in coll)
                                    formContent.Add(new StringContent(item?.ToString() ?? ""), propName);
                            }
                            else if (propVal is IFormFile nestedFile)
                            {
                                var nestedStream = nestedFile.OpenReadStream();
                                var nestedSc = new StreamContent(nestedStream);
                                nestedSc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                {
                                    Name = $"\"{propName}\"",
                                    FileName = $"\"{nestedFile.FileName}\""
                                };
                                nestedSc.Headers.ContentType = MediaTypeHeaderValue.Parse(nestedFile.ContentType);
                                formContent.Add(nestedSc, propName, nestedFile.FileName);
                            }
                            else if (propVal is Stream nestedStream)
                            {
                                var nestedSc = new StreamContent(nestedStream);
                                nestedSc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                {
                                    Name = $"\"{propName}\"",
                                    FileName = "\"file\""
                                };
                                formContent.Add(nestedSc, propName, "file");
                            }
                            else if (propVal is byte[] nestedBytes)
                            {
                                var nestedBc = new ByteArrayContent(nestedBytes);
                                nestedBc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                {
                                    Name = $"\"{propName}\"",
                                    FileName = "\"file\""
                                };
                                formContent.Add(nestedBc, propName, "file");
                            }
                            else if (propVal is HttpContent nestedHc)
                            {
                                formContent.Add(nestedHc, propName);
                            }
                            else
                            {
                                formContent.Add(new StringContent(JsonConvert.SerializeObject(propVal)), propName);
                            }
                        }
                    }
                    continue;
                }

                if (p.GetCustomAttribute<FromQueryAttribute>() != null)
                {
                    if (val is System.Collections.IEnumerable coll && val is not string)
                    {
                        foreach (var item in coll)
                            queryBuilder.Add(name, item?.ToString() ?? "");
                    }
                    else if (RequestHelper.IsPrimitive(val.GetType()))
                    {
                        queryBuilder.Add(name, val.ToString()!);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot bind complex object '{name}' of type '{val.GetType().Name}' to query string. Use [FromBody] or flatten your DTO into primitives.");
                    }
                    continue;
                }

                if (p.GetCustomAttribute<FromHeaderAttribute>() != null)
                {
                    headerParams[name] = val.ToString()!;
                    continue;
                }

                if (p.GetCustomAttribute<FromBodyAttribute>() != null)
                {
                    explicitBody = (name, val);
                    continue;
                }

                if (HttpMethods.IsGet(method))
                {
                    if (val is IEnumerable<object> coll && val is not string)
                    {
                        foreach (var item in coll)
                            queryBuilder.Add(name, item?.ToString() ?? "");
                    }
                    else
                    {
                        queryBuilder.Add(name, val.ToString()!);
                    }
                    continue;
                }

                defaultParams.Add((name, val));
            }

            foreach (var kv in routeParams)
                uri = uri.Replace("{" + kv.Key + "}", Uri.EscapeDataString(kv.Value));

            if (!HttpMethods.IsGet(method) && formContent != null)
            {
                content = formContent;
            }
            else if (!HttpMethods.IsGet(method))
            {
                if (explicitBody.HasValue)
                {
                    var value = explicitBody.Value.Value;
                    switch (value)
                    {
                        case HttpContent hc: content = hc; break;
                        case Stream s:
                            content = new StreamContent(s);
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            break;
                        case byte[] bytes: content = new ByteArrayContent(bytes); break;
                        case string str: content = new StringContent(str, Encoding.UTF8, "text/plain"); break;
                        default: content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"); break;
                    }
                    if (content.Headers.ContentType == null)
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                else
                {
                    var primitives = defaultParams.Where(x => RequestHelper.IsPrimitive(x.Value.GetType()));
                    foreach (var p in primitives)
                    {
                        if (p.Value is System.Collections.IEnumerable coll && p.Value is not string)
                        {
                            foreach (var item in coll)
                                queryBuilder.Add(p.Name, item?.ToString() ?? "");
                        }
                        else
                        {
                            queryBuilder.Add(p.Name, p.Value.ToString()!);
                        }
                    }
                    var complex = defaultParams.Where(x => !RequestHelper.IsPrimitive(x.Value.GetType())).ToList();
                    if (complex.Count > 1)
                        throw new InvalidOperationException("Cannot serialize multiple complex parameters to body; please use a single DTO or [FromBody] explicitly.");
                    else if (complex.Count == 1)
                    {
                        var value = complex[0].Value;
                        switch (value)
                        {
                            case HttpContent hc: content = hc; break;
                            case Stream s:
                                content = new StreamContent(s);
                                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                break;
                            case byte[] bytes: content = new ByteArrayContent(bytes); break;
                            case string str: content = new StringContent(str, Encoding.UTF8, "text/plain"); break;
                            default: content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"); break;
                        }
                        if (content.Headers.ContentType == null)
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }
                }
            }

            if (queryBuilder.Count() > 0)
                uri += queryBuilder.ToQueryString();

            return (method, uri, content, headerParams);
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
