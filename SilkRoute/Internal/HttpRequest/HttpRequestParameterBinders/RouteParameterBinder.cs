using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class RouteParameterBinder : AttributeParameterBinder<FromRouteAttribute>
{
    public override int Priority => 3;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        httpRequestBuilder.RouteParams[parameterInfo.Name!] = value.ToString()!;
    }
}