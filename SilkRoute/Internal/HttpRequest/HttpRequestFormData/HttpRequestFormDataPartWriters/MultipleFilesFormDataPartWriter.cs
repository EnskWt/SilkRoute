using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class MultipleFilesFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 10;

    public bool CanWritePart(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value is IFormFileCollection || value is IEnumerable<IFormFile>;
    }

    public void WritePart(
        HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

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

        var files = value as IFormFileCollection ?? (IEnumerable<IFormFile>)value;

        foreach (var file in files)
        {
            context.AddPart(form, name, file);
        }
    }
}