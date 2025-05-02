using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.HttpResponse;

namespace SilkRoute.Internal.HttpResponse.HttpResponseContentReaders;

internal sealed class VoidContentReader : IHttpResponseContentReader
{
    public int Priority => 10;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        return descriptor.ActionReturnTypeIndicatesVoid();
    }

    public Task<object> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        return Task.FromResult<object>(null);
    }
}