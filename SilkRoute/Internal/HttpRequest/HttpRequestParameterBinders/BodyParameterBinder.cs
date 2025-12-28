using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class BodyParameterBinder : AttributeParameterBinder<FromBodyAttribute>
{
    public override int Priority => 4;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        httpRequestBuilder.ExplicitBody = (parameterInfo.Name!, value);
    }
}