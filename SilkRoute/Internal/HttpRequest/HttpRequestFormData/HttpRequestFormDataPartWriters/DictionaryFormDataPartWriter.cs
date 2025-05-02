using System.Collections;
using SilkRoute.Internal.Abstractions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class DictionaryFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 60;

    public bool CanWritePart(object value)
    {
        return value is IDictionary;
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        var dict = (IDictionary)value;

        foreach (DictionaryEntry entry in dict)
        {
            var key = entry.Key.ToString();
            context.AddPart(form, $"{name}[{key}]", entry.Value);
        }
    }
}