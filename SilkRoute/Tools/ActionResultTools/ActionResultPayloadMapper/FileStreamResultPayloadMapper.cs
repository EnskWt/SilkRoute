using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper
{
    internal sealed class FileStreamResultPayloadMapper : IActionResultPayloadMapper
    {
        public int Priority => 20;

        public bool CanMap(HttpResponseMessage response, object? payload) => payload is Stream;

        public IActionResult Map(HttpResponseMessage response, object? payload)
        {
            var stream = (Stream)payload!;
            var contentType = response.Content?.Headers.ContentType?.ToString() ?? "application/octet-stream";

            var result = new FileStreamResult(stream, contentType);

            var fileName = response.Content?.Headers.ContentDisposition?.FileNameStar
                           ?? response.Content?.Headers.ContentDisposition?.FileName;

            if (!string.IsNullOrWhiteSpace(fileName))
                result.FileDownloadName = fileName;

            return result;
        }
    }

}
