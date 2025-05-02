using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class ByteArrayFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 40;

    public bool CanWritePart(object value)
    {
        return value is byte[];
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        form.Add(new ByteArrayContent((byte[])value), name);
    }
}