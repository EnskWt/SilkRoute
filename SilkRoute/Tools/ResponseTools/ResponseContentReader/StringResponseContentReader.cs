using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;
using SilkRoute.Tools.ResponseTools.ResponseExtensions;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader;

internal sealed class StringResponseContentReader : IResponseContentReader
{
    public int Priority => 30;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeMatchesString())
        {
            return true;
        }

        if (descriptor.GetActionReturnType().IsAbstractActionResultType())
        {
            return responseMessage.IsTextLikeMediaType();
        }

        return false;
    }


    public async Task<object?> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        return await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);
    }
}