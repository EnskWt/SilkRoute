using System.Collections;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.Common;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class EnumerableFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 70;

    public bool CanWritePart(object value)
    {
        return value is IEnumerable;
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        var i = 0;

        foreach (var item in (IEnumerable)value)
        {
            var isScalar = item is null || item is string || item.GetType().IsSimpleScalarType();
            var itemName = isScalar ? name : $"{name}[{i}]";

            context.AddPart(form, itemName, item);
            i++;
        }
    }
}