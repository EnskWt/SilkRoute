using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Extensions.ModelBinding;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal sealed class HeaderParameterBinder : AttributeParameterBinder<FromHeaderAttribute>
{
    public override int Priority => 10;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var headerParameterName = parameterInfo.GetModelBindingNameOrDefault<FromHeaderAttribute>();

        httpRequestBuilder.Headers[headerParameterName] = value.ToString()!;
    }
}