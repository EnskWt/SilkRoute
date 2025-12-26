using SilkRoute.Tools.ResponseTools.ResponseContentReader;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools;

internal sealed class ResponseReader
{
    private readonly List<IResponseContentReader> _contentReaders;

    public ResponseReader()
    {
        _contentReaders = new List<IResponseContentReader>
            {
                new VoidResponseContentReader(),
                new StreamResponseContentReader(),
                new ByteArrayResponseContentReader(),
                new StringResponseContentReader(),
                new JsonResponseContentReader()
            }
            .OrderBy(r => r.Priority)
            .ToList();
    }

    public async Task<object?> ReadResponseContent(
        HttpResponseMessage response,
        Type responseType,
        Type payloadType,
        bool isActionResult,
        CancellationToken cancellationToken = default)
    {
        foreach (var reader in _contentReaders)
        {
            if (reader.CanRead(responseType, payloadType, isActionResult, response))
            {
                return await reader.ReadAsync(response, responseType, payloadType, isActionResult, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        return null;
    }
}