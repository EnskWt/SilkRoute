using SilkRoute.Internal.HttpRequest.HttpRequestFormData;

namespace SilkRoute.Internal.Abstractions.HttpRequest;

internal interface IHttpRequestFormDataPartWriter
{
    int Priority { get; }
    bool CanWritePart(object value);

    void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value);
}