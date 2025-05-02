using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.ActionResult.ActionResultMappers;
using SilkRoute.Internal.Extensions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class AbstractActionResultWrapper : IActionResultWrapper
{
    private readonly IReadOnlyList<IActionResultMapper> _mappers;

    public AbstractActionResultWrapper()
    {
        _mappers = new List<IActionResultMapper>
            {
                new StatusCodeResultMapper(),
                new FileContentResultMapper(),
                new FileStreamResultMapper(),
                new ContentResultMapper(),
                new ObjectResultMapper()
            }
            .OrderBy(m => m.Priority)
            .ToList();
    }

    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType.IsAbstractActionResultType();
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        var mapper = _mappers.FirstOrDefault(m => m.CanMap(actionReturnValue));
        if (mapper is not null)
        {
            return mapper.Map(response, actionReturnValue);
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();

        throw new NotSupportedException(
            $"No payload mapper matched for abstract action result '{actionReturnType.FullName}'. " +
            $"Status={(int)response.StatusCode} ({response.StatusCode}), " +
            $"ContentType={response.Content.Headers.ContentType?.ToString() ?? "<null>"}, " +
            $"HasContentDisposition={response.Content.Headers.ContentDisposition is not null}");
    }
}