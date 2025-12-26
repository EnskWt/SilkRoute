using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper;

internal sealed class ObjectResultPayloadMapper : IActionResultPayloadMapper
{
    public int Priority => int.MaxValue;

    public bool CanMap(HttpResponseMessage response, object? payload) => payload is not null;

    public object Map(HttpResponseMessage response, object? payload)
    {
        var statusCode = (int)response.StatusCode;
        var contentType = response.Content?.Headers.ContentType?.ToString();

        var result = new ObjectResult(payload)
        {
            StatusCode = statusCode
        };

        if (!string.IsNullOrWhiteSpace(contentType))
            result.ContentTypes.Add(contentType);

        return result;
    }
}