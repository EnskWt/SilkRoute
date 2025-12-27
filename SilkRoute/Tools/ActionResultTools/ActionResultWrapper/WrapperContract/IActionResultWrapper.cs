using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

internal interface IActionResultWrapper
{
    int Priority { get; }

    bool CanWrap(IActionReturnDescriptor actionReturnDescriptor);

    object Wrap(HttpResponseMessage response, IActionReturnDescriptor actionReturnDescriptor, object? actionReturnValue);
}