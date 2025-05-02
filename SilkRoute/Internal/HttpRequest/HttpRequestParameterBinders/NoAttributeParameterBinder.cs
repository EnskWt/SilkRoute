using System.Reflection;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal sealed class NoAttributeParameterBinder : IHttpRequestParameterBinder
{
    public int Priority => int.MaxValue;

    public bool CanBind(ParameterInfo parameterInfo, object value)
    {
        return value is not null;
    }

    public void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        httpRequestBuilder.NoAttributeParams.Add((parameterInfo.Name!, value));
    }
}