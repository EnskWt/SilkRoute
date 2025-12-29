using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class FileContentResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return typeof(FileContentResult).IsAssignableFrom(actionReturnType);
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

        if (actionReturnValue is not byte[] bytes)
        {
            throw new InvalidOperationException(
                $"FileContentResult requires byte[] value, got '{actionReturnValue?.GetType().Name ?? "null"}'.");
        }

        var contentType = response.Content?.Headers?.ContentType?.ToString() ?? "application/octet-stream";

        var result = new FileContentResult(bytes, contentType);

        var fileName =
            response.Content?.Headers?.ContentDisposition?.FileNameStar ??
            response.Content?.Headers?.ContentDisposition?.FileName;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            result.FileDownloadName = fileName;
        }

        return result;
    }
}