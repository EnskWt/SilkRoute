using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader
{
    internal sealed class JsonResponseContentReader : IResponseContentReader
    {
        public int Priority => int.MaxValue;

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

            if (!isActionResult)
                return true;

            if (isGenericActionResult)
                return true;

            if (typeof(ObjectResult).IsAssignableFrom(responseType))
                return true;

            if (!string.IsNullOrEmpty(mediaType) &&
                mediaType.Contains("json", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public async Task<object?> ReadAsync(
            HttpResponseMessage response,
            Type responseType,
            Type payloadType,
            bool isActionResult,
            CancellationToken cancellationToken = default)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var targetType =
                isActionResult && (typeof(IActionResult).IsAssignableFrom(payloadType)
                   || typeof(IConvertToActionResult).IsAssignableFrom(payloadType))
                    ? typeof(ExpandoObject)
                    : payloadType;

            var settings = targetType == typeof(ExpandoObject)
                ? new JsonSerializerSettings { Converters = { new ExpandoObjectConverter() } }
                : null;

            return settings == null
                ? JsonConvert.DeserializeObject(json, targetType)
                : JsonConvert.DeserializeObject(json, targetType, settings);
        }
    }
}
