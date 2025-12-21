using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper
{
    internal sealed class StatusCodeResultPayloadMapper : IActionResultPayloadMapper
    {
        public int Priority => 0;

        public bool CanMap(HttpResponseMessage response, object? payload) => payload is null;

        public object Map(HttpResponseMessage response, object? payload)
            => new StatusCodeResult((int)response.StatusCode);
    }
}
