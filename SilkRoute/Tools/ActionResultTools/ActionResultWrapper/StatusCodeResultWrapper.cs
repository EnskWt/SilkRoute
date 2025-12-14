using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper
{
    internal sealed class StatusCodeResultWrapper : IActionResultWrapper
    {
        public int Priority => 40;

        public bool CanWrap(Type responseType)
            => typeof(StatusCodeResult).IsAssignableFrom(responseType);

        public IActionResult Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            var statusCode = (int)response.StatusCode;

            return (StatusCodeResult)Activator.CreateInstance(typeof(StatusCodeResult), statusCode)!;
        }
    }
}
