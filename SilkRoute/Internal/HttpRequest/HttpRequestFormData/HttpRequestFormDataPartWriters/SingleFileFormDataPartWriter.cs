using System.Net.Http.Headers;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class SingleFileFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 10;

    public bool CanWritePart(object value)
    {
        return value is IFormFile;
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        var file = (IFormFile)value;

        var stream = file.OpenReadStream();
        var content = new StreamContent(stream);

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        form.Add(content, name, file.FileName);
    }
}