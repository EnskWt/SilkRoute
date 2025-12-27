using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ResponseTools.ResponseContentReader;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools;

internal sealed class ResponseReader
{
    private readonly HttpResponseMessage _response;
    private readonly IActionReturnDescriptor _actionReturnDescriptor;
    private readonly IReadOnlyList<IResponseContentReader> _contentReaders;

    public ResponseReader(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
        _actionReturnDescriptor = actionReturnDescriptor ?? throw new ArgumentNullException(nameof(actionReturnDescriptor));
        
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

    public async Task<object?> ReadResponseContent()
    {
        foreach (var reader in _contentReaders)
        {
            if (reader.CanRead(_response, _actionReturnDescriptor))
            {
                return await reader.ReadAsync(_response, _actionReturnDescriptor)
                    .ConfigureAwait(false);
            }
        }

        return null;
    }
}