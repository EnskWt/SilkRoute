using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader
{
    internal sealed class VoidResponseContentReader : IResponseContentReader
    {
        public int Priority => 0;

        public bool CanRead(
            Type responseType,
            Type payloadType,
            bool isActionResult,
            HttpResponseMessage response)
        {
            return payloadType == typeof(void);
        }

        public Task<object?> ReadAsync(
            HttpResponseMessage response,
            Type responseType,
            Type payloadType,
            bool isActionResult,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object?>(null);
        }
    }
}
