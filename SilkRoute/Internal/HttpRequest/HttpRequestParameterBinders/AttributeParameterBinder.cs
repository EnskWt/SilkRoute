using System.Reflection;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal abstract class AttributeParameterBinder<T> : IHttpRequestParameterBinder where T : Attribute
{
    public abstract int Priority { get; }

    public bool CanBind(ParameterInfo parameterInfo, object? value)
    {
        if (value == null)
        {
            return false;
        }

        return parameterInfo.GetCustomAttribute<T>() != null;
    }

    public abstract void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value);
}