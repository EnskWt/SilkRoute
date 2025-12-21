using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader
{
    internal sealed class StringResponseContentReader : IResponseContentReader
    {
        public int Priority => 30;

        public bool CanRead(
            Type responseType,
            Type payloadType,
            bool isActionResult,
            HttpResponseMessage response)
        {
            var mediaType = response.Content?.Headers.ContentType?.MediaType;

            bool isAbstractActionResult =
                isActionResult &&
                !responseType.IsGenericType &&
                (responseType == typeof(IActionResult) || responseType == typeof(ActionResult));

            bool isGenericActionResult =
                isActionResult &&
                responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(ActionResult<>);

            bool isConcreteActionResult =
                isActionResult && !isAbstractActionResult && !isGenericActionResult;

            if (payloadType == typeof(string))
                return true;

            if (isActionResult && typeof(ContentResult).IsAssignableFrom(responseType))
                return true;

            if (isActionResult && (isAbstractActionResult || isConcreteActionResult) && !string.IsNullOrEmpty(mediaType))
            {
                if (mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (mediaType.Contains("xml", StringComparison.OrdinalIgnoreCase)
                    || mediaType.Contains("html", StringComparison.OrdinalIgnoreCase))
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
            return await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
