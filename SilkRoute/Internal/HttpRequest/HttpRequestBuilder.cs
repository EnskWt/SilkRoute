using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.Extensions.HttpRequest;
using SilkRoute.Internal.HttpRequest.HttpRequestContentBuilders;
using SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

namespace SilkRoute.Internal.HttpRequest;

internal sealed class HttpRequestBuilder
{
    private readonly IReadOnlyList<IHttpRequestContentBuilder> _contentBuilders;
    private readonly IReadOnlyList<IHttpRequestParameterBinder> _parameterBinders;
    
    private readonly HttpMethod _httpMethod;
    private readonly string _uriTemplate;
    private readonly ParameterInfo[] _parameterInfos;
    private readonly object[] _parameterValues;

    internal HttpRequestBuilder(HttpMethod method,
        string uriTemplate,
        ParameterInfo[] parameterInfos,
        object[] parameterValues)
    {
        _httpMethod = method;
        _uriTemplate = uriTemplate;
        _parameterInfos = parameterInfos;
        _parameterValues = parameterValues;

        _contentBuilders = new List<IHttpRequestContentBuilder>
        {
            new NoAttributeParameterContentBuilder(),
            new FormParameterContentBuilder(),
            new ExplicitBodyContentBuilder()
        }.OrderBy(x => x.Priority).ToList();

        _parameterBinders = new List<IHttpRequestParameterBinder>
        {
            new FormParameterBinder(),
            new HeaderParameterBinder(),
            new BodyParameterBinder(),
            new QueryParameterBinder(),
            new RouteParameterBinder(),
            new NoAttributeParameterBinder()
        }.OrderBy(x => x.Priority).ToList();
    }

    public Dictionary<string, string> RouteParams { get; } = new();
    public QueryBuilder QueryBuilder { get; } = new();
    public List<(string Name, object Value)> FormParams { get; } = new();
    public Dictionary<string, string> Headers { get; } = new();
    public (string Name, object Value)? ExplicitBody { get; set; }
    public List<(string Name, object Value)> NoAttributeParams { get; } = new();

    public void EnsureNoBodyAndFormDataConflict()
    {
        if (ExplicitBody.HasValue && FormParams.Count > 0)
        {
            throw new InvalidOperationException(
                "Using both [FromBody] and [FromForm] attributes in one request is not supported.");
        }

        if (ExplicitBody?.Value is not null && ExplicitBody.Value.Value.ContainsNonExplicitFormData())
        {
            throw new InvalidOperationException(
                $"Parameter '{ExplicitBody.Value.Name}' marked as [FromBody], but contains form-data fields. Use [FromForm] instead.");
        }

        if (ExplicitBody.HasValue && NoAttributeParams.Any(p => p.Value.ContainsNonExplicitFormData()))
        {
            throw new InvalidOperationException(
                "Cannot mix a [FromBody] parameter with attributeless parameters that contain form-data fields.");
        }

        if (FormParams.Count > 0 && NoAttributeParams.Any(p =>
                !p.Value.GetType().IsSimpleScalarType() && !p.Value.ContainsNonExplicitFormData()))
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

        if (ExplicitBody.HasValue || FormParams.Count != 0)
        {
            return;
        }
        
        var nonExplicitBodyParams = NoAttributeParams
            .Where(p => !p.Value.GetType().IsSimpleScalarType() && !p.Value.ContainsNonExplicitFormData()).ToList();

        var nonExplicitFormParams = NoAttributeParams
            .Where(p => p.Value.ContainsNonExplicitFormData())
            .ToList();

        if (nonExplicitBodyParams.Any() && nonExplicitFormParams.Any())
        {
            throw new InvalidOperationException(
                "Mixing attributeless form-data parameters with attributeless complex-object parameters in the same request is not supported. " +
                "Please annotate form-data parameters with [FromForm] or complex object parameters with [FromBody].");
        }
    }

    public void BindAllParameters()
    {
        BindRouteParametersFromTemplate();
        BindParametersByAttribute();
    }

    private void BindRouteParametersFromTemplate()
    {
        var templateParamNames = _uriTemplate.ExtractRouteParameters();

        for (var i = 0; i < _parameterInfos.Length; i++)
        {
            var p = _parameterInfos[i];
            var value = _parameterValues is not null && i < _parameterValues.Length ? _parameterValues[i] : null;
            if (value is null)
            {
                continue;
            }

            var fromRoute = p.GetCustomAttribute<FromRouteAttribute>(true);

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

    private void BindParametersByAttribute()
    {
        if (_parameterValues is null || _parameterValues.Length == 0 || _parameterInfos.Length == 0)
        {
            return;
        }

        for (var i = 0; i < _parameterInfos.Length; i++)
        {
            var parameter = _parameterInfos[i];
            var value = _parameterValues is not null && i < _parameterValues.Length ? _parameterValues[i] : null;

            if (value is null)
            {
                continue;
            }

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

    public (string method, string uri, HttpContent content, IDictionary<string, string> headers) BuildRequest()
    {
        HttpContent content = null;

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

        var uri = _uriTemplate!;
        uri = uri.ApplyRouteValues(RouteParams);

        if (QueryBuilder.Any())
        {
            uri += QueryBuilder.ToQueryString();
        }

        return (_httpMethod.Method, uri, content, Headers);
    }
}