using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData;

internal sealed class HttpRequestFormDataPartWriterContext
{
    private readonly IReadOnlyList<IHttpRequestFormDataPartWriter> _writers;

    public HttpRequestFormDataPartWriterContext(IEnumerable<IHttpRequestFormDataPartWriter> writers)
    {
        if (writers is null)
        {
            throw new ArgumentNullException(nameof(writers));
        }

        _writers = writers
            .OrderBy(w => w.Priority)
            .ToList();
    }

    public void AddPart(MultipartFormDataContent form, string name, object value)
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
            return;
        }

        var writer = _writers.FirstOrDefault(x => x.CanWritePart(value));
        if (writer is null)
        {
            throw new InvalidOperationException(
                $"No form-data writer can handle value of type '{value.GetType().FullName}' for field '{name}'.");
        }

        writer.WritePart(this, form, name, value);
    }
}
