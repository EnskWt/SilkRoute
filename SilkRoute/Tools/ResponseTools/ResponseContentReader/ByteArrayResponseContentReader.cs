using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;
using SilkRoute.Tools.ResponseTools.ResponseExtensions;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader;

internal sealed class ByteArrayResponseContentReader : IResponseContentReader
{
    public int Priority => 20;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeMatchesByteArray())
        {
            return true;
        }

        if (descriptor.GetActionReturnType().IsAbstractActionResultType())
        {
            return responseMessage.HasContentDisposition()
                   || responseMessage.IsFileMediaType();
        }

        return false;
    }

    public async Task<object?> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        return await response.Content.ReadAsByteArrayAsync()
            .ConfigureAwait(false);
    }
}