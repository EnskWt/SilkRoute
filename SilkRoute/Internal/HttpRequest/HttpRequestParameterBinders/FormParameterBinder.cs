using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class FormParameterBinder : AttributeParameterBinder<FromFormAttribute>
{
    public override int Priority => 50;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var fromForm = parameterInfo.GetCustomAttribute<FromFormAttribute>(inherit: true);

        var formParameterName =
            !string.IsNullOrWhiteSpace(fromForm?.Name)
                ? fromForm.Name!
                : parameterInfo.Name!;
        
        httpRequestBuilder.FormParams.Add((formParameterName, value));
    }
}