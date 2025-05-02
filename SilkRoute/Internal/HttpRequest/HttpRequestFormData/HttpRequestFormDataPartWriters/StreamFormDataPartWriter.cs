using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class StreamFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 30;

    public bool CanWritePart(object value)
    {
        return value is Stream;
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        form.Add(new StreamContent((Stream)value), name);
    }
}