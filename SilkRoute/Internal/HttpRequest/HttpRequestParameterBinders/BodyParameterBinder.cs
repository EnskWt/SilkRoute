using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal sealed class BodyParameterBinder : AttributeParameterBinder<FromBodyAttribute>
{
    public override int Priority => 40;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        httpRequestBuilder.ExplicitBody = (parameterInfo.Name!, value);
    }
}