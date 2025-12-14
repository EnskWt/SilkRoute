using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper
{
    internal sealed class DefaultActionResultWrapper : IActionResultWrapper
    {
        public int Priority => int.MaxValue;

        private readonly List<IActionResultPayloadMapper> _mappers;

        public DefaultActionResultWrapper()
        {
            _mappers = new IActionResultPayloadMapper[]
            {
                new StatusCodeResultPayloadMapper(),
                new FileContentResultPayloadMapper(),
                new FileStreamResultPayloadMapper(),
                new ContentResultPayloadMapper(),
                new ObjectResultPayloadMapper()
            }
            .OrderBy(m => m.Priority)
            .ToList();
        }

        public bool CanWrap(Type responseType)
        {
            if (responseType == typeof(IActionResult) || responseType == typeof(ActionResult))
                return true;

            return typeof(IActionResult).IsAssignableFrom(responseType)
                   || typeof(IConvertToActionResult).IsAssignableFrom(responseType);
        }

        public IActionResult Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            var mapper = _mappers.FirstOrDefault(m => m.CanMap(response, payload));
            if (mapper != null)
                return mapper.Map(response, payload);

            return new ContentResult
            {
                Content = payload?.ToString(),
                ContentType = response.Content?.Headers.ContentType?.ToString() ?? "text/plain",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
