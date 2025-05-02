using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultMappers;

internal sealed class ObjectResultMapper : IActionResultMapper
{
    public int Priority => int.MaxValue;

    public bool CanMap(object payload)
    {
        return payload is not null;
    }

    public object Map(HttpResponseMessage response, object payload)
    {
        var statusCode = (int)response.StatusCode;
        var contentType = response.Content.Headers.ContentType?.ToString();

        var result = new ObjectResult(payload)
        {
            StatusCode = statusCode
        };

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            result.ContentTypes.Add(contentType);
        }

        return result;
    }
}