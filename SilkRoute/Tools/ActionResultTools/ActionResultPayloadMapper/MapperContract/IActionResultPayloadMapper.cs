using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract
{
    internal interface IActionResultPayloadMapper
    {
        int Priority { get; }
        bool CanMap(HttpResponseMessage response, object? payload);
        object Map(HttpResponseMessage response, object? payload);
    }
}
