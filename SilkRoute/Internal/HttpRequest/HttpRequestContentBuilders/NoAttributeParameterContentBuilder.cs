using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.HttpRequest.HttpRequestBodySerializers;
using SilkRoute.Internal.HttpRequest.HttpRequestFormData;
using SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

namespace SilkRoute.Internal.HttpRequest.HttpRequestContentBuilders;

internal sealed class NoAttributeParameterContentBuilder : IHttpRequestContentBuilder
{
    private readonly IReadOnlyList<IHttpRequestBodySerializer> _bodySerializers;
    private readonly HttpRequestFormDataPartWriterContext _formContext;

    public NoAttributeParameterContentBuilder()
    {
        _bodySerializers = new List<IHttpRequestBodySerializer>
        {
            new StreamBodySerializer(),
            new ByteArrayBodySerializer(),
            new StringBodySerializer(),
            new JsonBodySerializer()
        }.OrderBy(x => x.Priority).ToList();

        var partWriters = new List<IHttpRequestFormDataPartWriter>
        {
            new SingleFileFormDataPartWriter(),
            new MultipleFilesFormDataPartWriter(),
            new SimpleScalarContentFormDataPartWriter(),
            new EnumerableContentFormDataPartWriter(),
            new ComplexContentFormDataPartWriter()
        };

        _formContext = new HttpRequestFormDataPartWriterContext(partWriters);
    }

    public int Priority => int.MaxValue;

    public bool CanBuild(HttpRequestBuilder httpRequestBuilder)
    {
        if (httpRequestBuilder is null)
        {
            throw new ArgumentNullException(nameof(httpRequestBuilder));
        }

        return httpRequestBuilder.NoAttributeParams.Count > 0;
    }

    public HttpContent? Build(HttpRequestBuilder httpRequestBuilder)
    {
        if (httpRequestBuilder is null)
        {
            throw new ArgumentNullException(nameof(httpRequestBuilder));
        }

        if (CanBuildForm(httpRequestBuilder))
        {
            return BuildForm(httpRequestBuilder);
        }

        if (CanBuildBody(httpRequestBuilder))
        {
            return BuildBody(httpRequestBuilder);
        }

        return null;
    }

    private static bool CanBuildForm(HttpRequestBuilder httpRequestBuilder)
    {
        return httpRequestBuilder.NoAttributeParams.Any(p => p.Value.ContainsNonExplicitFormData());
    }

    private HttpContent BuildForm(HttpRequestBuilder httpRequestBuilder)
    {
        var items = httpRequestBuilder.NoAttributeParams
            .Where(p => p.Value.ContainsNonExplicitFormData())
            .ToList();

        foreach (var p in items)
        {
            httpRequestBuilder.NoAttributeParams.Remove(p);
        }

        var form = new MultipartFormDataContent();

        foreach (var (name, value) in items)
        {
            _formContext.AddPart(form, name, value);
        }

        return form;
    }

    private static bool CanBuildBody(HttpRequestBuilder httpRequestBuilder)
    {
        return httpRequestBuilder.NoAttributeParams.Any(p =>
            !p.Value.GetType().IsSimpleScalarType() &&
            !p.Value.ContainsNonExplicitFormData());
    }

    private HttpContent BuildBody(HttpRequestBuilder httpRequestBuilder)
    {
        var firstBodyParam = httpRequestBuilder.NoAttributeParams.First(p =>
            !p.Value.GetType().IsSimpleScalarType() &&
            !p.Value.ContainsNonExplicitFormData());

        httpRequestBuilder.NoAttributeParams.Remove(firstBodyParam);

        var value = firstBodyParam.Value;

        if (value.ContainsNonExplicitFormData() || value.ContainsNestedStream())
        {
            throw new InvalidOperationException(
                $"Parameter of type '{value.GetType().Name}' contains a Stream or Form-Data. " +
                "You must pass streams either as top-level [FromBody] (Stream) or [FromForm] (IFormFile), " +
                "but not embed them in a complex DTO.");
            // TODO: message
        }

        var serializer = _bodySerializers.First(x => x.CanSerialize(value));
        return serializer.Serialize(value);
    }
}