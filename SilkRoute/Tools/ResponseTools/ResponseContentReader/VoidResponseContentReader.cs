using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader;

internal sealed class VoidResponseContentReader : IResponseContentReader
{
    public int Priority => 0;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        return descriptor.ActionReturnTypeMatchesVoid();
    }

    public Task<object?> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        return Task.FromResult<object?>(null);
    }
}