using System.Globalization;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class SimpleScalarContentFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 20;

    public bool CanWritePart(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.GetType().IsSimpleScalarType();
    }

    public void WritePart(
        HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
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
            throw new ArgumentNullException(nameof(value));
        }

        var str = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        form.Add(new StringContent(str), name);
    }
}
