using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using SilkRoute.Internal.Constants;

namespace SilkRoute.Public.InputFormatters;

public sealed class BinaryBodyInputFormatter : InputFormatter
{
    public BinaryBodyInputFormatter()
    {
        foreach (var mt in MediaTypeConstants.FileMediaTypes)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mt));
        }

        foreach (var prefix in MediaTypeConstants.FileMediaTypePrefixes)
        {
            var wildcard = prefix.TrimEnd('/') + "/*";
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(wildcard));
        }
    }

    protected override bool CanReadType(Type type)
    {
        return type == typeof(Stream) || type == typeof(byte[]);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        var abort = context.HttpContext.RequestAborted;

        if (context.ModelType == typeof(byte[]))
        {
            using var ms = new MemoryStream();
            await request.Body.CopyToAsync(ms, abort);
            return await InputFormatterResult.SuccessAsync(ms.ToArray());
        }

        if (context.ModelType == typeof(Stream))
        {
            var ms = new MemoryStream();
            await request.Body.CopyToAsync(ms, abort);
            ms.Position = 0;
            return await InputFormatterResult.SuccessAsync(ms);
        }

        return await InputFormatterResult.FailureAsync();
    }
}