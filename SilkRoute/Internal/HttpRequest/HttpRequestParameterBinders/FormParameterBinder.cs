using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Extensions.ModelBinding;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal sealed class FormParameterBinder : AttributeParameterBinder<FromFormAttribute>
{
    public override int Priority => 50;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var formParameterName = parameterInfo.GetModelBindingNameOrDefault<FromFormAttribute>();

        httpRequestBuilder.FormParams.Add((formParameterName, value));
    }
}