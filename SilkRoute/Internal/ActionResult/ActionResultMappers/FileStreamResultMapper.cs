using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultMappers;

internal sealed class FileStreamResultMapper : IActionResultMapper
{
    public int Priority => 20;

    public bool CanMap(object payload)
    {
        return payload is Stream;
    }

    public object Map(HttpResponseMessage response, object payload)
    {
        var stream = (Stream)payload!;
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var result = new FileStreamResult(stream, contentType);

        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            result.FileDownloadName = fileName;
        }

        return result;
    }
}