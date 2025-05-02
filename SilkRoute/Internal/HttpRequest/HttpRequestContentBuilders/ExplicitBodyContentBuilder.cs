using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;

namespace SilkRoute.Internal.HttpRequest.HttpRequestContentBuilders;

internal sealed class ExplicitBodyContentBuilder : IHttpRequestContentBuilder
{
    private readonly IReadOnlyList<IHttpRequestBodySerializer> _bodySerializers;

    public ExplicitBodyContentBuilder()
    {
        _bodySerializers = new List<IHttpRequestBodySerializer>
        {
            new StreamBodySerializer(),
            new ByteArrayBodySerializer(),
            new StringBodySerializer(),
            new JsonBodySerializer()
        }.OrderBy(x => x.Priority).ToList();
    }

    public int Priority => 10;

    public bool CanBuild(HttpRequestBuilder httpRequestBuilder)
    {
        return httpRequestBuilder.ExplicitBody.HasValue;
    }

    public HttpContent Build(HttpRequestBuilder httpRequestBuilder)
    {
        var value = httpRequestBuilder.ExplicitBody!.Value.Value;

        if (value.ContainsNonExplicitFormData() || value.ContainsNestedStream())
        {
            throw new InvalidOperationException(
                $"Parameter of type '{value.GetType().Name}' contains a nested Stream and/or form-data (e.g., IFormFile). " +
                "Streams and files cannot be embedded inside a complex JSON DTO. " +
                "Pass binary content as a top-level [FromBody] Stream/byte[] (raw body) " +
                "or send files using multipart/form-data and bind them as top-level [FromForm] IFormFile / IFormFileCollection. " +
                "Do not nest Stream/IFormFile/form-data inside other objects.");
        }

        var serializer = _bodySerializers.First(x => x.CanSerialize(value));
        return serializer.Serialize(value);
    }
}