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
            .OrderBy(x => x.Priority)
            .ToList();
    }

    public void AddPart(MultipartFormDataContent form, string name, object? value)
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

        var writer = _writers.First(x => x.CanWritePart(value));
        writer.WritePart(this, form, name, value);
    }
}