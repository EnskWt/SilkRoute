using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper;
using SilkRoute.Tools.ActionResultTools.ActionResultPayloadMapper.MapperContract;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

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

    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType.IsAbstractActionResultType();
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object? actionReturnValue)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var mapper = _mappers.FirstOrDefault(m => m.CanMap(response, actionReturnValue));
        if (mapper != null)
        {
            return mapper.Map(response, actionReturnValue);
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();

        throw new NotSupportedException(
            $"No payload mapper matched for abstract action result '{actionReturnType.FullName}'. " +
            $"Status={(int)response.StatusCode} ({response.StatusCode}), " +
            $"ContentType={response.Content.Headers.ContentType?.ToString() ?? "<null>"}, " +
            $"HasContentDisposition={(response.Content.Headers.ContentDisposition != null)}");
    }
}