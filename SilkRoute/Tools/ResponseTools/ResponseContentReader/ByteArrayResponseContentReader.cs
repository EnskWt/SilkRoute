using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader
{
    internal sealed class ByteArrayResponseContentReader : IResponseContentReader
    {
        public int Priority => 20;

        public bool CanRead(
            Type responseType,
            Type payloadType,
            bool isActionResult,
            HttpResponseMessage response)
        {
            var mediaType = response.Content?.Headers.ContentType?.MediaType;
            var hasContentDisposition = response.Content?.Headers.ContentDisposition != null;

            bool isAbstractActionResult =
                isActionResult &&
                !responseType.IsGenericType &&
                (responseType == typeof(IActionResult) || responseType == typeof(ActionResult));

            if (payloadType == typeof(byte[]))
                return true;

            if (isActionResult &&
                typeof(FileContentResult).IsAssignableFrom(responseType))
                return true;

            if (isAbstractActionResult)
            {
                if (hasContentDisposition)
                    return true;

                if (!string.IsNullOrEmpty(mediaType)
                    && !mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
                    && !mediaType.Contains("json", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public async Task<object?> ReadAsync(
            HttpResponseMessage response,
            Type responseType,
            Type payloadType,
            bool isActionResult,
            CancellationToken cancellationToken = default)
        {
            return await response.Content.ReadAsByteArrayAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
