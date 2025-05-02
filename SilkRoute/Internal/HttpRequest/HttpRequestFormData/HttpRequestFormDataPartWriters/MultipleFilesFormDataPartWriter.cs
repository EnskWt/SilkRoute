using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class MultipleFilesFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 20;

    public bool CanWritePart(object value)
    {
        return value is IFormFileCollection or IEnumerable<IFormFile>;
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        var files = value as IFormFileCollection ?? (IEnumerable<IFormFile>)value;

        foreach (var file in files)
        {
            context.AddPart(form, name, file);
        }
    }
}