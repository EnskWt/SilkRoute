using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools;

internal sealed class ActionResultWrapperFactory
{
    private readonly List<IActionResultWrapper> _actionResultWrappers;

    public ActionResultWrapperFactory()
    {
        _actionResultWrappers = new IActionResultWrapper[]
            {
                new GenericActionResultWrapper(),
                new ObjectResultWrapper(),
                new FileStreamResultWrapper(),
                new FileContentResultWrapper(),
                new StatusCodeResultWrapper(),
                new ContentResultWrapper(),
                new AbstractActionResultWrapper()
            }
            .OrderBy(w => w.Priority)
            .ToList();
    }

    public object Wrap(HttpResponseMessage response, Type responseType, object? payload)
    {
        foreach (var wrapper in _actionResultWrappers)
        {
            if (wrapper.CanWrap(responseType))
                return wrapper.Wrap(response, responseType, payload);
        }

        throw new NotSupportedException(
            $"No ActionResult wrapper registered for '{responseType.FullName}'. " +
            $"Status={(int)response.StatusCode} ({response.StatusCode}), " +
            $"ContentType={response.Content?.Headers.ContentType?.ToString() ?? "<null>"}.");
    }
}