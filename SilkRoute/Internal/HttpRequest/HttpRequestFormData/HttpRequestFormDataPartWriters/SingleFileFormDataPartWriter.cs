using System.Net.Http.Headers;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class SingleFileFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public bool CanWritePart(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value is IFormFile;
    }

    public void WritePart(
        HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        if (form is null)
        {
            throw new ArgumentNullException(nameof(form));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Form field name cannot be null or whitespace.", nameof(name));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var file = (IFormFile)value;

        using var ms = new MemoryStream();
        file.CopyTo(ms);

        var bytes = ms.ToArray();
        var content = new ByteArrayContent(bytes);

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        form.Add(content, name, file.FileName);
    }
}