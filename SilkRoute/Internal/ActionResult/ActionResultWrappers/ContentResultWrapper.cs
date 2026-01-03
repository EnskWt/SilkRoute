using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class ContentResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return typeof(ContentResult).IsAssignableFrom(actionReturnType);
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        var statusCode = (int)response.StatusCode;
        var contentType = response.Content.Headers.ContentType?.ToString();

        return new ContentResult
        {
            StatusCode = statusCode,
            ContentType = contentType,
            Content = actionReturnValue?.ToString()
        };
    }
}