using System.Text;
using Newtonsoft.Json;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

internal class JsonBodySerializer : IHttpRequestBodySerializer
{
    public int Priority => int.MaxValue;
    public bool CanSerialize(object val) => true;
    public HttpContent Serialize(object val) => new StringContent(JsonConvert.SerializeObject(val), Encoding.UTF8, "application/json");
}