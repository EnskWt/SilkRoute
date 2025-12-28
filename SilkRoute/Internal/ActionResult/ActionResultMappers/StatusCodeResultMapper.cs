using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultMappers;

internal sealed class StatusCodeResultMapper : IActionResultMapper
{
    public int Priority => 0;

    public bool CanMap(object? payload) => payload is null;

    public object Map(HttpResponseMessage response, object? payload)
    {
        return new StatusCodeResult((int)response.StatusCode);
    }
}