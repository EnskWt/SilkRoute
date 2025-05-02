using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData;

internal sealed class HttpRequestFormDataPartWriterContext
{
    private readonly IReadOnlyList<IHttpRequestFormDataPartWriter> _writers;
    
    private readonly HashSet<object> _visiting = new(ReferenceEqualityComparer.Instance);

    public HttpRequestFormDataPartWriterContext()
    {
        _writers = new List<IHttpRequestFormDataPartWriter>
        {
            new SingleFileFormDataPartWriter(),
            new MultipleFilesFormDataPartWriter(),
            new StreamFormDataPartWriter(),
            new ByteArrayFormDataPartWriter(),
            new SimpleScalarFormDataPartWriter(),
            new DictionaryFormDataPartWriter(),
            new EnumerableFormDataPartWriter(),
            new ComplexFormDataPartWriter()
        }.OrderBy(x => x.Priority).ToList();
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

        if (!_visiting.Add(value))
        {
            throw new InvalidOperationException(
                $"Cycle detected while writing form field '{name}' for type '{value.GetType().FullName}'.");
        }

        var writer = _writers.First(x => x.CanWritePart(value));
        writer.WritePart(this, form, name, value);

        _visiting.Remove(value);
    }
}