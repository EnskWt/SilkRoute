using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class StatusCodeResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType == typeof(StatusCodeResult);
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        var statusCode = (int)response.StatusCode;
        return new StatusCodeResult(statusCode);
    }
}