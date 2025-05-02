using System.Globalization;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class SimpleScalarFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 50;

    public bool CanWritePart(object value)
    {
        return value.GetType().IsSimpleScalarType();
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        var str = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        form.Add(new StringContent(str), name);
    }
}