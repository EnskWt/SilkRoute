using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using SilkRoute.Tools.RequestTools.RequestContentBinder;
using SilkRoute.Tools.RequestTools.RequestContentBinder.BinderContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;
using SilkRoute.Tools.RequestTools.RequestParametersBinders;
using SilkRoute.Tools.RequestTools.RequestParametersBinders.BinderContract;

namespace SilkRoute.Tools.RequestTools
{
    internal class RequestBuilder
    {
        public HttpMethod? Method { get; }
        public string? UriTemplate { get; }

        internal Dictionary<string, string> RouteParams { get; } = new();
        internal QueryBuilder QueryBuilder { get; } = new();
        internal List<(string Name, object Value)> FormParams { get; } = new();
        internal Dictionary<string, string> Headers { get; } = new();
        internal (string Name, object Value)? ExplicitBody { get; set; }
        internal List<(string Name, object Value)> NoAttributeParams { get; } = new();


        private readonly List<IRequestContentBinder> _contentBinders;
        private readonly List<IRequestParametersBinder> _parameterBinders;

        internal RequestBuilder(HttpMethod method, string uriTemplate)
        {
            Method = method;
            UriTemplate = uriTemplate;

            _contentBinders = new List<IRequestContentBinder>()
            {
                new NoAttributeContentBinder(),
                new FormParametersContentBinder(),
                new ExplicitBodyContentBinder()
            }.OrderBy(x => x.Priority).ToList();

            _parameterBinders = new List<IRequestParametersBinder>()
            {
                new FormParametersBinder(),
                new HeaderParametersBinder(),
                new BodyParametersBinder(),
                new QueryParametersBinder(),
                new RouteParametersBinder(),
                new NoAttributeParametersBinder()
            }.OrderBy(x => x.Priority).ToList();
        }

        internal void EnsureNoBodyAndFormDataConflict()
        {
            if (ExplicitBody.HasValue && FormParams.Count > 0)
            {
                throw new InvalidOperationException(
                    "SilkRoute doesn't support using both [FromBody] and [FromForm] attributes in one request.");
            }

            if (ExplicitBody?.Value != null && RequestTypeHelper.ContainsNonExplicitFormData(ExplicitBody.Value.Value))
            {
                throw new InvalidOperationException(
                    $"Parameter '{ExplicitBody.Value.Name}' marked as [FromBody], but contains form-data fields. Use [FromForm] instead.");
            }

            if (ExplicitBody.HasValue && NoAttributeParams.Any(p => RequestTypeHelper.ContainsNonExplicitFormData(p.Value)))
            {
                throw new InvalidOperationException(
                    $"Cannot mix a [FromBody] parameter with attributeless parameters that contain form-data fields.");
            }

            if (FormParams.Count > 0 && NoAttributeParams.Any(p => !RequestTypeHelper.IsPrimitive(p.Value.GetType()) && !RequestTypeHelper.ContainsNonExplicitFormData(p.Value)))
            {
                var conflict = NoAttributeParams
                    .Where(p => !RequestTypeHelper.IsPrimitive(p.Value.GetType()) && !RequestTypeHelper.ContainsNonExplicitFormData(p.Value))
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
                    .Where(p => !RequestTypeHelper.IsPrimitive(p.Value.GetType()) && !RequestTypeHelper.ContainsNonExplicitFormData(p.Value)).ToList();

                var nonExplicitFormParams = NoAttributeParams
                    .Where(p => RequestTypeHelper.ContainsNonExplicitFormData(p.Value))
                    .ToList();

                if (nonExplicitBodyParams.Any() && nonExplicitFormParams.Any())
                {
                    throw new InvalidOperationException(
                        "SilkRoute doesn't support mixing attributeless form-data parameters with attributeless complex-object parameters in the same request. " +
                        "Please annotate from-data parameters with [FromForm] or complex object parameters with [FromBody].");
                }
            }
        }

        internal void BindRouteParametersFromTemplate(ParameterInfo[] parameters, object?[]? args)
        {
            if (UriTemplate == null) return;

            var templateRouteParams = RouteParameterHelper.ExtractRouteParametersFromTemplate(UriTemplate)
                .Select(ph => (ph.Name, ph.Type))
                .ToList();

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                var value = args != null && i < args.Length ? args[i] : null;
                if (value == null) continue;

                var routeTemplateType = RouteParameterHelper.ConvertToRouteTemplateType(p.ParameterType);
                bool matches = templateRouteParams
                    .Any(ph =>
                        ph.Name == p.Name
                        && (ph.Type == null || ph.Type == routeTemplateType));

                if (matches)
                    RouteParams[p.Name!] = value.ToString()!;
            }
        }

        internal void BindParametersByAttribute(ParameterInfo[] parameters, object?[]? args)
        {
            if (args == null || args.Length == 0 || parameters.Length == 0) return;

            for (int i = 0; i < parameters.Length; i++)
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

            foreach (var binder in _contentBinders)
            {
                if (binder.CanBind(this))
                {
                    content = binder.Bind(this);
                    break;
                }
            }

            foreach (var p in NoAttributeParams)
            {
                QueryParameterHelper.AddQueryParams(QueryBuilder, p.Name, p.Value);
            }

            var uri = UriTemplate!;
            foreach (var kv in RouteParams)
                uri = uri.Replace("{" + kv.Key + "}", Uri.EscapeDataString(kv.Value));

            if (QueryBuilder.Count() > 0)
                uri += QueryBuilder.ToQueryString();

            return (Method!.Method, uri, content, Headers);
        }
    }
}
