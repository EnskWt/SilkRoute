using System.Collections;
using System.Reflection;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

using System.Reflection;

internal sealed class ComplexContentFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 50;

    public bool CanWritePart(object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return !value.GetType().IsSimpleScalarType()
               && value is not IEnumerable
               && value is not IFormFile;
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

        foreach (var prop in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            var pv = prop.GetValue(value);
            if (pv is null)
            {
                continue;
            }

            context.AddPart(form, $"{name}.{prop.Name}", pv);
        }
    }
}
