using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class ObjectResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType == typeof(ObjectResult);
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        var statusCode = (int)response.StatusCode;
        var contentType = response.Content.Headers.ContentType?.ToString();

        var obj = new ObjectResult(actionReturnValue)
        {
            StatusCode = statusCode
        };

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            obj.ContentTypes.Clear();
            obj.ContentTypes.Add(contentType);
        }

        return obj;
    }
}