using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class FileStreamResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType == typeof(FileStreamResult);
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        if (actionReturnValue is not Stream stream)
        {
            throw new InvalidOperationException(
                $"FileStreamResult requires Stream value, got '{actionReturnValue?.GetType().Name ?? "null"}'.");
        }

        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var result = new FileStreamResult(stream, contentType);

        var fileName =
            response.Content.Headers.ContentDisposition?.FileNameStar ??
            response.Content.Headers.ContentDisposition?.FileName;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            result.FileDownloadName = fileName;
        }

        return result;
    }
}