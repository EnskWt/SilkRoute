using System.Text;
using Newtonsoft.Json;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

internal class StringBodySerializer : IHttpRequestBodySerializer
{
    public int Priority => 30;
    public bool CanSerialize(object val) => val is string;
    public HttpContent Serialize(object val) => new StringContent(JsonConvert.SerializeObject((string)val), Encoding.UTF8, "application/json");
}