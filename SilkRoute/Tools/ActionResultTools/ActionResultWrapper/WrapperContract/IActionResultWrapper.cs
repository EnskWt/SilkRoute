using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract
{
    internal interface IActionResultWrapper
    {
        int Priority { get; }

        bool CanWrap(Type responseType);

        IActionResult Wrap(
            HttpResponseMessage response,
            Type responseType,
            object? payload);
    }
}
