using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultMappers;

internal sealed class FileContentResultMapper : IActionResultMapper
{
    public int Priority => 30;

    public bool CanMap(object payload)
    {
        return payload is byte[];
    }

    public object Map(HttpResponseMessage response, object payload)
    {
        var bytes = (byte[])payload!;
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var result = new FileContentResult(bytes, contentType);

        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            result.FileDownloadName = fileName;
        }

        return result;
    }
}