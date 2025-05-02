using System.Reflection;
using SilkRoute.Internal.Abstractions.Common;
using SilkRoute.Internal.HttpRequest;

namespace SilkRoute.Internal.Abstractions.HttpRequest;

internal interface IHttpRequestParameterBinder : IPrioritized
{
    bool CanBind(ParameterInfo parameterInfo, object value);
    void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value);
}