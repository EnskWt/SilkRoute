using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.Contract;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper;

internal sealed class AbstractActionResultWrapper : IActionResultWrapper
{
    public int Priority => int.MaxValue;

    private readonly List<IActionResultPayloadMapper> _mappers;

    public AbstractActionResultWrapper()
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
        return responseType == typeof(IActionResult) || responseType == typeof(ActionResult);
    }

    public object Wrap(HttpResponseMessage response, Type responseType, object? payload)
    {
        var mapper = _mappers.FirstOrDefault(m => m.CanMap(response, payload));
        if (mapper != null)
            return mapper.Map(response, payload);

        throw new NotSupportedException(
            $"No payload mapper matched for abstract action result. " +
            $"Status={(int)response.StatusCode} ({response.StatusCode}), " +
            $"ContentType={response.Content?.Headers.ContentType?.ToString() ?? "<null>"}, " +
            $"HasContentDisposition={(response.Content?.Headers.ContentDisposition != null)}");
    }
}