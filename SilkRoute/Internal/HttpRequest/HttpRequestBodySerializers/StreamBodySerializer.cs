using System.Net.Http.Headers;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

internal sealed class StreamBodySerializer : IHttpRequestBodySerializer
{
    public int Priority => 10;

    public bool CanSerialize(object val)
    {
        return val is Stream;
    }

    public HttpContent Serialize(object val)
    {
        var s = (Stream)val;
        var sc = new StreamContent(s);
        sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return sc;
    }
}