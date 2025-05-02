using System.Text;
using Newtonsoft.Json;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

internal sealed class StringBodySerializer : IHttpRequestBodySerializer
{
    public int Priority => 30;

    public bool CanSerialize(object val)
    {
        return val is string;
    }

    public HttpContent Serialize(object val)
    {
        return new StringContent(JsonConvert.SerializeObject((string)val), Encoding.UTF8, "application/json");
    }
}