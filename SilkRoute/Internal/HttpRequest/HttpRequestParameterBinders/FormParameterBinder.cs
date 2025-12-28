using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class FormParameterBinder : AttributeParameterBinder<FromFormAttribute>
{
    public override int Priority => 5;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        httpRequestBuilder.FormParams.Add((parameterInfo.Name!, value));
    }
}