using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools
{
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
                new DefaultActionResultWrapper()
            }
            .OrderBy(w => w.Priority)
            .ToList();
        }

        public IActionResult Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            foreach (var wrapper in _actionResultWrappers)
            {
                if (wrapper.CanWrap(responseType))
                    return wrapper.Wrap(response, responseType, payload);
            }

            return new ContentResult
            {
                Content = payload?.ToString(),
                ContentType = response.Content?.Headers.ContentType?.ToString() ?? "text/plain",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
