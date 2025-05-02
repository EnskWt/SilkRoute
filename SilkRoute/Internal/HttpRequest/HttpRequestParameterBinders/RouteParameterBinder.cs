using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Extensions.ModelBinding;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal sealed class RouteParameterBinder : AttributeParameterBinder<FromRouteAttribute>
{
    public override int Priority => 30;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var routeParameterName = parameterInfo.GetModelBindingNameOrDefault<FromRouteAttribute>();

        httpRequestBuilder.RouteParams[routeParameterName] = value.ToString()!;
    }
}