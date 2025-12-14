using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper
{
    internal sealed class FileContentResultPayloadMapper : IActionResultPayloadMapper
    {
        public int Priority => 10;

        public bool CanMap(HttpResponseMessage response, object? payload) => payload is byte[];

        public IActionResult Map(HttpResponseMessage response, object? payload)
        {
            var bytes = (byte[])payload!;
            var contentType = response.Content?.Headers.ContentType?.ToString() ?? "application/octet-stream";

            var result = new FileContentResult(bytes, contentType);

            var fileName = response.Content?.Headers.ContentDisposition?.FileNameStar
                           ?? response.Content?.Headers.ContentDisposition?.FileName;

            if (!string.IsNullOrWhiteSpace(fileName))
                result.FileDownloadName = fileName;

            return result;
        }
    }
}
