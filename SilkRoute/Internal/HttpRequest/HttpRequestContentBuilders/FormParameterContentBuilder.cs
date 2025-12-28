using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.HttpRequest.HttpRequestFormData;
using SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

namespace SilkRoute.Internal.HttpRequest.HttpRequestContentBuilders;

internal sealed class FormParameterContentBuilder : IHttpRequestContentBuilder
{
    private readonly HttpRequestFormDataPartWriterContext _formContext;

    public FormParameterContentBuilder()
    {
        var partWriters = new List<IHttpRequestFormDataPartWriter>
        {
            new SingleFileFormDataPartWriter(),
            new MultipleFilesFormDataPartWriter(),
            new SimpleScalarContentFormDataPartWriter(),
            new EnumerableContentFormDataPartWriter(),
            new ComplexContentFormDataPartWriter()
        }.OrderBy(x => x.Priority).ToList();
        
        _formContext = new HttpRequestFormDataPartWriterContext(partWriters);
    }

    public int Priority => 1;

    public bool CanBuild(HttpRequestBuilder httpRequestBuilder)
    {
        if (httpRequestBuilder is null)
        {
            throw new ArgumentNullException(nameof(httpRequestBuilder));
        }

        return httpRequestBuilder.FormParams.Count > 0;
    }

    public HttpContent Build(HttpRequestBuilder httpRequestBuilder)
    {
        if (httpRequestBuilder is null)
        {
            throw new ArgumentNullException(nameof(httpRequestBuilder));
        }

        foreach (var p in httpRequestBuilder.NoAttributeParams
                     .Where(p => p.Value.ContainsNonExplicitFormData())
                     .ToList())
        {
            httpRequestBuilder.FormParams.Add(p);
            httpRequestBuilder.NoAttributeParams.Remove(p);
        }

        var form = new MultipartFormDataContent();

        foreach (var (name, value) in httpRequestBuilder.FormParams)
        {
            _formContext.AddPart(form, name, value);
        }

        return form;
    }
}