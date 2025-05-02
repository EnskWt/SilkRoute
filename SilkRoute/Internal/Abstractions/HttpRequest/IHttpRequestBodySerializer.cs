using SilkRoute.Internal.Abstractions.Common;

namespace SilkRoute.Internal.Abstractions.HttpRequest;

internal interface IHttpRequestBodySerializer : IPrioritized
{
    bool CanSerialize(object val);
    HttpContent Serialize(object val);
}