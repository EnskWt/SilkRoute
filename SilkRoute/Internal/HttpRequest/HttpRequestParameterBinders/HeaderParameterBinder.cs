using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class HeaderParameterBinder : AttributeParameterBinder<FromHeaderAttribute>
{
    public override int Priority => 1;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var fromHeader = parameterInfo.GetCustomAttribute<FromHeaderAttribute>(inherit: true);

        var headerName =
            !string.IsNullOrWhiteSpace(fromHeader?.Name)
                ? fromHeader!.Name!
                : parameterInfo.Name!;

        httpRequestBuilder.Headers[headerName] = value.ToString()!;
    }
}