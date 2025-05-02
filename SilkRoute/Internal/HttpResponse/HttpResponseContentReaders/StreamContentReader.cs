using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.HttpResponse;
using SilkRoute.Internal.Extensions.ActionResult;
using SilkRoute.Internal.Extensions.HttpResponse;

namespace SilkRoute.Internal.HttpResponse.HttpResponseContentReaders;

internal sealed class StreamContentReader : IHttpResponseContentReader
{
    public int Priority => 20;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeIndicatesStream())
        {
            if (responseMessage.HasMediaType() && !responseMessage.IsFileMediaType() &&
                !responseMessage.HasContentDisposition())
            {
                return false;
            }

            return true;
        }

        if (descriptor.GetActionReturnType().IsAbstractActionResultType())
        {
            return responseMessage.IsFileMediaType()
                   || responseMessage.HasContentDisposition();
        }

        return false;
    }

    public async Task<object> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        return await response.Content.ReadAsStreamAsync()
            .ConfigureAwait(false);
    }
}