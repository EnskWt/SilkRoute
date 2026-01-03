using System.Collections;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class DictionaryContentFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 30;

    public bool CanWritePart(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value is IDictionary;
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

        var dict = (IDictionary)value;

        foreach (DictionaryEntry entry in dict)
        {
            if (entry.Value is null)
            {
                continue;
            }

            var key = entry.Key.ToString();
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }
            
            var fieldName = $"{name}[{key}]";
            context.AddPart(form, fieldName, entry.Value);
        }
    }
}
