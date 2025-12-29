using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class HeaderParameterBinder : AttributeParameterBinder<FromHeaderAttribute>
{
    public override int Priority => 10;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var fromHeader = parameterInfo.GetCustomAttribute<FromHeaderAttribute>(inherit: true);

        var headerParameterName =
            !string.IsNullOrWhiteSpace(fromHeader?.Name)
                ? fromHeader.Name!
                : parameterInfo.Name!;

        httpRequestBuilder.Headers[headerParameterName] = value.ToString()!;
    }
}