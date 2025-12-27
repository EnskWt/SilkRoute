using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;
using SilkRoute.Tools.ResponseTools.ResponseExtensions;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader;

internal sealed class StreamResponseContentReader : IResponseContentReader
{
    public int Priority => 10;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeMatchesStream())
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
        return await response.Content.ReadAsStreamAsync()
            .ConfigureAwait(false);
    }
}