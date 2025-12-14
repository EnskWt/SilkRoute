using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper
{
    internal sealed class FileContentResultWrapper : IActionResultWrapper
    {
        public int Priority => 30;

        public bool CanWrap(Type responseType)
            => typeof(FileContentResult).IsAssignableFrom(responseType);

        public IActionResult Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            if (payload is not byte[] bytes)
                throw new InvalidOperationException($"FileContentResult requires byte[] payload, got '{payload?.GetType().Name ?? "null"}'.");

            var contentType = response.Content?.Headers.ContentType?.ToString() ?? "application/octet-stream";

            var result = new FileContentResult(bytes, contentType);

            var fileName =
                response.Content?.Headers.ContentDisposition?.FileNameStar ??
                response.Content?.Headers.ContentDisposition?.FileName;

            if (!string.IsNullOrWhiteSpace(fileName))
                result.FileDownloadName = fileName;

            return result;
        }
    }
}
