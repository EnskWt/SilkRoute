using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;


internal sealed class ObjectResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return typeof(ObjectResult).IsAssignableFrom(actionReturnType);
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object? actionReturnValue)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();

        var statusCode = (int)response.StatusCode;
        var contentType = response.Content?.Headers?.ContentType?.ToString();

        var obj = (ObjectResult)Activator.CreateInstance(actionReturnType, actionReturnValue)!;

        obj.StatusCode = statusCode;

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            obj.ContentTypes.Clear();
            obj.ContentTypes.Add(contentType);
        }

        if (obj.Value == null && actionReturnValue != null)
        {
            obj.Value = actionReturnValue;
        }

        return obj;
    }
}