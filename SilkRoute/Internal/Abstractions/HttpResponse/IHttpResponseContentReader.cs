using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.Common;

namespace SilkRoute.Internal.Abstractions.HttpResponse;

internal interface IHttpResponseContentReader : IPrioritized
{
    bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor);

    Task<object> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor);
}