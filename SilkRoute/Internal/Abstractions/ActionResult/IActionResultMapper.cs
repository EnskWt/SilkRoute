using SilkRoute.Internal.Abstractions.Common;

namespace SilkRoute.Internal.Abstractions.ActionResult;

internal interface IActionResultMapper : IPrioritized
{
    bool CanMap(object payload);
    object Map(HttpResponseMessage response, object payload);
}