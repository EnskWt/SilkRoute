using System.Net.Http.Headers;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

internal sealed class ByteArrayBodySerializer : IHttpRequestBodySerializer
{
    public int Priority => 20;

    public bool CanSerialize(object val)
    {
        return val is byte[];
    }

    public HttpContent Serialize(object val)
    {
        var b = (byte[])val;
        var bac = new ByteArrayContent(b);
        bac.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return bac;
    }
}