using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            if (isActionResult &&
                (!responseType.IsGenericType
                 || responseType.GetGenericTypeDefinition() != typeof(ActionResult<>)))
            {
                return false;
            }


            return true;
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

            return JsonConvert.DeserializeObject(json, payloadType);
        }
    }
}
