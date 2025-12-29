using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.Common;

namespace SilkRoute.Internal.Abstractions.ActionResult;

internal interface IActionResultWrapper
{
    bool CanWrap(IActionReturnDescriptor actionReturnDescriptor);

    object Wrap(HttpResponseMessage response, IActionReturnDescriptor actionReturnDescriptor, object? actionReturnValue);
}