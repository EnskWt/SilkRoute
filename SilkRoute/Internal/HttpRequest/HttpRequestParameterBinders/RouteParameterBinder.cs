using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class RouteParameterBinder : AttributeParameterBinder<FromRouteAttribute>
{
    public override int Priority => 30;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var fromRoute = parameterInfo.GetCustomAttribute<FromRouteAttribute>(inherit: true);

        var routeParameterName =
            !string.IsNullOrWhiteSpace(fromRoute?.Name)
                ? fromRoute.Name!
                : parameterInfo.Name!;
        
        httpRequestBuilder.RouteParams[routeParameterName] = value.ToString()!;
    }
}