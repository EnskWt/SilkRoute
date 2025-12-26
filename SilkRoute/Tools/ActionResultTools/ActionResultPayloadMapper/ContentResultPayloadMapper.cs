using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper;

internal sealed class ContentResultPayloadMapper : IActionResultPayloadMapper
{
    public int Priority => 30;

    public bool CanMap(HttpResponseMessage response, object? payload) => payload is string;

    public object Map(HttpResponseMessage response, object? payload)
    {
        var contentType = response.Content?.Headers.ContentType?.ToString() ?? "text/plain";

        return new ContentResult
        {
            Content = (string)payload!,
            ContentType = contentType,
            StatusCode = (int)response.StatusCode
        };
    }
}