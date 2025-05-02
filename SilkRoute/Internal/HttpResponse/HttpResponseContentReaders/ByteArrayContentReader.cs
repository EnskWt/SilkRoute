using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.HttpResponse;
using SilkRoute.Internal.Extensions.ActionResult;
using SilkRoute.Internal.Extensions.HttpResponse;

namespace SilkRoute.Internal.HttpResponse.HttpResponseContentReaders;

internal sealed class ByteArrayContentReader : IHttpResponseContentReader
{
    public int Priority => 30;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeIndicatesByteArray())
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
        return await response.Content.ReadAsByteArrayAsync()
            .ConfigureAwait(false);
    }
}