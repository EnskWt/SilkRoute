using System.Text;
using Newtonsoft.Json;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

internal sealed class JsonBodySerializer : IHttpRequestBodySerializer
{
    public int Priority => int.MaxValue;

    public bool CanSerialize(object val)
    {
        return true;
    }

    public HttpContent Serialize(object val)
    {
        return new StringContent(JsonConvert.SerializeObject(val), Encoding.UTF8, "application/json");
    }
}