using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.HttpResponse;
using SilkRoute.Internal.Extensions.ActionResult;
using SilkRoute.Internal.Extensions.HttpResponse;

namespace SilkRoute.Internal.HttpResponse.HttpResponseContentReaders;

internal sealed class StringContentReader : IHttpResponseContentReader
{
    public int Priority => 40;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeIndicatesString())
        {
            return true;
        }

        if (descriptor.GetActionReturnType().IsAbstractActionResultType())
        {
            return responseMessage.IsTextLikeMediaType();
        }

        return false;
    }

    public async Task<object> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        return await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);
    }
}