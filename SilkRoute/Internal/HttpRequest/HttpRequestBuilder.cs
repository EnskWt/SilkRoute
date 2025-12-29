using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.Extensions.HttpRequest;
using SilkRoute.Internal.HttpRequest.HttpRequestContentBuilders;
using SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

namespace SilkRoute.Internal.HttpRequest;

// TODO: передавати параметри в конструктор
internal class HttpRequestBuilder
{
    public HttpMethod? Method { get; }
    public string? UriTemplate { get; }

    internal Dictionary<string, string> RouteParams { get; } = new();
    internal QueryBuilder QueryBuilder { get; } = new();
    internal List<(string Name, object Value)> FormParams { get; } = new();
    internal Dictionary<string, string> Headers { get; } = new();
    internal (string Name, object Value)? ExplicitBody { get; set; }
    internal List<(string Name, object Value)> NoAttributeParams { get; } = new();


    private readonly List<IHttpRequestContentBuilder> _contentBuilders;
    private readonly List<IHttpRequestParameterBinder> _parameterBinders;

    internal HttpRequestBuilder(HttpMethod method, string uriTemplate)
    {
        Method = method;
        UriTemplate = uriTemplate;

        _contentBuilders = new List<IHttpRequestContentBuilder>()
        {
            new NoAttributeParameterContentBuilder(),
            new FormParameterContentBuilder(),
            new ExplicitBodyContentBuilder()
        }.OrderBy(x => x.Priority).ToList();

        _parameterBinders = new List<IHttpRequestParameterBinder>()
        {
            new FormParameterBinder(),
            new HeaderParameterBinder(),
            new BodyParameterBinder(),
            new QueryParameterBinder(),
            new RouteParameterBinder(),
            new NoAttributeParameterBinder()
        }.OrderBy(x => x.Priority).ToList();
    }

    internal void EnsureNoBodyAndFormDataConflict()
    {
        if (ExplicitBody.HasValue && FormParams.Count > 0)
        {
            throw new InvalidOperationException(
                "SilkRoute doesn't support using both [FromBody] and [FromForm] attributes in one request.");
        }

        if (ExplicitBody?.Value != null && ExplicitBody.Value.Value.ContainsNonExplicitFormData())
        {
            throw new InvalidOperationException(
                $"Parameter '{ExplicitBody.Value.Name}' marked as [FromBody], but contains form-data fields. Use [FromForm] instead.");
        }

        if (ExplicitBody.HasValue && NoAttributeParams.Any(p => p.Value.ContainsNonExplicitFormData()))
        {
            throw new InvalidOperationException(
                $"Cannot mix a [FromBody] parameter with attributeless parameters that contain form-data fields.");
        }

        if (FormParams.Count > 0 && NoAttributeParams.Any(p => !p.Value.GetType().IsSimpleScalarType() && !p.Value.ContainsNonExplicitFormData()))
        {
            var conflict = NoAttributeParams
                .Where(p => !p.Value.GetType().IsSimpleScalarType() && !p.Value.ContainsNonExplicitFormData())
                .Select(p => p.Name)
                .ToList();

            throw new InvalidOperationException(
                $"Cannot mix [FromForm] parameters with complex, non-attributed parameters ({string.Join(", ", conflict)}). " +
                "Complex object parameters without [FromForm] or [FromBody] would be bound as the request body. " +
                "Either remove [FromForm] or explicitly annotate those parameters.");
        }

        if (!ExplicitBody.HasValue && FormParams.Count == 0)
        {
            var nonExplicitBodyParams = NoAttributeParams
                .Where(p => !p.Value.GetType().IsSimpleScalarType() && !p.Value.ContainsNonExplicitFormData()).ToList();

            var nonExplicitFormParams = NoAttributeParams
                .Where(p => p.Value.ContainsNonExplicitFormData())
                .ToList();

            if (nonExplicitBodyParams.Any() && nonExplicitFormParams.Any())
            {
                throw new InvalidOperationException(
                    "SilkRoute doesn't support mixing attributeless form-data parameters with attributeless complex-object parameters in the same request. " +
                    "Please annotate from-data parameters with [FromForm] or complex object parameters with [FromBody].");
            }
        }
    }

    private void BindRouteParametersFromTemplate(ParameterInfo[] parameters, object?[]? args)
    {
        if (UriTemplate == null)
        {
            return;
        }

        var templateParamNames = UriTemplate.ExtractRouteParameters();

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var value = args != null && i < args.Length ? args[i] : null;
            if (value == null)
            {
                continue;
            }
            
            var fromRoute = p.GetCustomAttribute<FromRouteAttribute>(inherit: true);

            var routeParameterName =
                !string.IsNullOrWhiteSpace(fromRoute?.Name)
                    ? fromRoute.Name!
                    : p.Name!;

            var matches = templateParamNames.Any(n =>
                string.Equals(n, routeParameterName, StringComparison.OrdinalIgnoreCase));

            if (matches)
            {
                RouteParams[routeParameterName] = value.ToString()!;
            }
        }
    }

    private void BindParametersByAttribute(ParameterInfo[] parameters, object?[]? args)
    {
        if (args == null || args.Length == 0 || parameters.Length == 0)
        {
            return;
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var value = args != null && i < args.Length ? args[i] : null;

            if (value == null) continue;
            foreach (var binder in _parameterBinders)
            {
                if (binder.CanBind(parameter, value))
                {
                    binder.Bind(this, parameter, value);
                    break;
                }
            }
        }
    }

    internal void BindAllParameters(ParameterInfo[] parameters, object?[]? args)
    {
        BindRouteParametersFromTemplate(parameters, args);
        BindParametersByAttribute(parameters, args);
    }

    internal (string method, string uri, HttpContent? content, IDictionary<string, string> headers) BuildRequest()
    {
        HttpContent? content = null;

        foreach (var binder in _contentBuilders)
        {
            if (binder.CanBuild(this))
            {
                content = binder.Build(this);
                break;
            }
        }

        foreach (var p in NoAttributeParams)
        {
            QueryBuilder.AddFlattenedParameter(p.Name, p.Value);
        }

        var uri = UriTemplate!;
        uri = uri.ApplyRouteValues(RouteParams);

        if (QueryBuilder.Any())
        {
            uri += QueryBuilder.ToQueryString();
        }

        return (Method!.Method, uri, content, Headers);
    }
}