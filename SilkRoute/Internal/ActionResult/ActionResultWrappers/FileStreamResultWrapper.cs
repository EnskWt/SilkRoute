using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class FileStreamResultWrapper : IActionResultWrapper
{
    public int Priority => 20;

    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return typeof(FileStreamResult).IsAssignableFrom(actionReturnType);
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

        if (actionReturnValue is not Stream stream)
        {
            throw new InvalidOperationException(
                $"FileStreamResult requires Stream value, got '{actionReturnValue?.GetType().Name ?? "null"}'.");
        }

        var contentType = response.Content?.Headers?.ContentType?.ToString() ?? "application/octet-stream";

        var result = new FileStreamResult(stream, contentType);

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