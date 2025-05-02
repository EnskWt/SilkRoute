using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class FileContentResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType == typeof(FileContentResult);
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        if (actionReturnValue is not byte[] bytes)
        {
            throw new InvalidOperationException(
                $"FileContentResult requires byte[] value, got '{actionReturnValue?.GetType().Name ?? "null"}'.");
        }

        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

        var result = new FileContentResult(bytes, contentType);

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