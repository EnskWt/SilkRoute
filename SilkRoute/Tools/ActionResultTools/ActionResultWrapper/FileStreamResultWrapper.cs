using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper
{
    internal sealed class FileStreamResultWrapper : IActionResultWrapper
    {
        public int Priority => 20;

        public bool CanWrap(Type responseType)
            => typeof(FileStreamResult).IsAssignableFrom(responseType);

        public IActionResult Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            if (payload is not Stream stream)
                throw new InvalidOperationException($"FileStreamResult requires Stream payload, got '{payload?.GetType().Name ?? "null"}'.");

            var contentType = response.Content?.Headers.ContentType?.ToString() ?? "application/octet-stream";

            var result = new FileStreamResult(stream, contentType);

            var fileName =
                response.Content?.Headers.ContentDisposition?.FileNameStar ??
                response.Content?.Headers.ContentDisposition?.FileName;

            if (!string.IsNullOrWhiteSpace(fileName))
                result.FileDownloadName = fileName;

            return result;
        }
    }
}
