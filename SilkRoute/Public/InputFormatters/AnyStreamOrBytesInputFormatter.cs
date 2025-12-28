using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace SilkRoute.Public.InputFormatters;

// TODO: Review
public sealed class AnyStreamOrBytesInputFormatter : InputFormatter
{
    public AnyStreamOrBytesInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/octet-stream"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/pdf"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/zip"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
    }

    protected override bool CanReadType(Type type)
        => type == typeof(Stream) || type == typeof(byte[]);

    public override bool CanRead(InputFormatterContext context)
    {
        if (!base.CanRead(context))
            return false;

        var ct = context.HttpContext.Request.ContentType;
        
        if (!string.IsNullOrWhiteSpace(ct))
        {
            if (ct.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (ct.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (ct.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return true; 
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