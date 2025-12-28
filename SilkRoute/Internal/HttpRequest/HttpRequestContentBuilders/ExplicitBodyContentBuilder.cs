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

    public int Priority => 0;

    public bool CanBuild(HttpRequestBuilder httpRequestBuilder)
    {
        if (httpRequestBuilder is null)
        {
            throw new ArgumentNullException(nameof(httpRequestBuilder));
        }

        return httpRequestBuilder.ExplicitBody.HasValue;
    }

    public HttpContent? Build(HttpRequestBuilder httpRequestBuilder)
    {
        if (httpRequestBuilder is null)
        {
            throw new ArgumentNullException(nameof(httpRequestBuilder));
        }

        var value = httpRequestBuilder.ExplicitBody!.Value.Value;

        if (value.ContainsNonExplicitFormData() || value.ContainsNestedStream())
        {
            throw new InvalidOperationException(
                $"Parameter of type '{value.GetType().Name}' contains a Stream or Form-Data. " +
                "You must pass streams either as top-level [FromBody] (Stream) or [FromForm] (IFormFile), " +
                "but not embed them in a complex DTO.");
            // TODO: Verify error message, maybe it's not only about stream
        }

        var serializer = _bodySerializers.First(x => x.CanSerialize(value));
        return serializer.Serialize(value);
    }
}