using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.HttpResponse;
using SilkRoute.Internal.HttpResponse.HttpResponseContentReaders;

namespace SilkRoute.Internal.HttpResponse;

internal sealed class HttpResponseReader
{
    private readonly HttpResponseMessage _response;
    private readonly IActionReturnDescriptor _actionReturnDescriptor;
    private readonly IReadOnlyList<IHttpResponseContentReader> _contentReaders;

    public HttpResponseReader(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
        _actionReturnDescriptor =
            actionReturnDescriptor ?? throw new ArgumentNullException(nameof(actionReturnDescriptor));

        _contentReaders = new List<IHttpResponseContentReader>
        {
            new VoidContentReader(),
            new StreamContentReader(),
            new ByteArrayContentReader(),
            new StringContentReader(),
            new JsonContentReader()
        }.OrderBy(x => x.Priority).ToList();
    }

    public async Task<object> ReadResponseContent()
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