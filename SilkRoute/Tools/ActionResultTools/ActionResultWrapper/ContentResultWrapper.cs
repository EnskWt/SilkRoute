using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper;

internal sealed class ContentResultWrapper : IActionResultWrapper
{
    public int Priority => 50;

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
        object? actionReturnValue)
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