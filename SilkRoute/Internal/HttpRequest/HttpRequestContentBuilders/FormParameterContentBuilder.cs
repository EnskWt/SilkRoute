using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.HttpRequest.HttpRequestFormData;

namespace SilkRoute.Internal.HttpRequest.HttpRequestContentBuilders;

internal sealed class FormParameterContentBuilder : IHttpRequestContentBuilder
{
    private readonly HttpRequestFormDataPartWriterContext _formContext;

    public FormParameterContentBuilder()
    {
        _formContext = new HttpRequestFormDataPartWriterContext();
    }

    public int Priority => 20;

    public bool CanBuild(HttpRequestBuilder httpRequestBuilder)
    {
        return httpRequestBuilder.FormParams.Count > 0;
    }

    public HttpContent Build(HttpRequestBuilder httpRequestBuilder)
    {
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