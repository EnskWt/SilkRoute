using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper;

internal sealed class JsonResultWrapper : IActionResultWrapper
{
    public int Priority => 15;

    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return typeof(JsonResult).IsAssignableFrom(actionReturnType);
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

        var statusCode = (int)response.StatusCode;
        var contentType = response.Content?.Headers?.ContentType?.ToString();

        var result = new JsonResult(actionReturnValue)
        {
            StatusCode = statusCode
        };

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            result.ContentType = contentType;
        }

        return result;
    }
}