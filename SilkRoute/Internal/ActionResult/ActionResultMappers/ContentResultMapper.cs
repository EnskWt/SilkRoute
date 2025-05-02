using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultMappers;

internal sealed class ContentResultMapper : IActionResultMapper
{
    public int Priority => 40;

    public bool CanMap(object payload)
    {
        return payload is string;
    }

    public object Map(HttpResponseMessage response, object payload)
    {
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "text/plain";

        return new ContentResult
        {
            Content = (string)payload!,
            ContentType = contentType,
            StatusCode = (int)response.StatusCode
        };
    }
}