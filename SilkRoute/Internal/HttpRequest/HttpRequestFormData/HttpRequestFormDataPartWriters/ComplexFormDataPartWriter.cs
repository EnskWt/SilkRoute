using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.HttpRequest;
using SilkRoute.Internal.Extensions.ModelBinding;

namespace SilkRoute.Internal.HttpRequest.HttpRequestFormData.HttpRequestFormDataPartWriters;

internal sealed class ComplexFormDataPartWriter : IHttpRequestFormDataPartWriter
{
    public int Priority => 80;

    public bool CanWritePart(object value)
    {
        return true;
    }

    public void WritePart(HttpRequestFormDataPartWriterContext context,
        MultipartFormDataContent form,
        string name,
        object value)
    {
        foreach (var prop in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            var pv = prop.GetValue(value);

            var propName = prop.GetModelBindingNameOrDefault<FromFormAttribute>();

            context.AddPart(form, $"{name}.{propName}", pv);
        }
    }
}