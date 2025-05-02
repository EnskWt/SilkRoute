using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.ActionResult.ActionResultWrappers;

namespace SilkRoute.Internal.ActionResult;

internal sealed class ActionResultWrapperDispatcher
{
    private readonly HttpResponseMessage _response;
    private readonly IActionReturnDescriptor _actionReturnDescriptor;
    private readonly object _actionReturnValue;
    private readonly IReadOnlyList<IActionResultWrapper> _actionResultWrappers;

    public ActionResultWrapperDispatcher(HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        _actionResultWrappers = new List<IActionResultWrapper>
        {
            new GenericActionResultWrapper(),
            new ObjectResultWrapper(),
            new JsonResultWrapper(),
            new FileStreamResultWrapper(),
            new FileContentResultWrapper(),
            new StatusCodeResultWrapper(),
            new ContentResultWrapper(),
            new AbstractActionResultWrapper()
        };

        _response = response ?? throw new ArgumentNullException(nameof(response));
        _actionReturnDescriptor =
            actionReturnDescriptor ?? throw new ArgumentNullException(nameof(actionReturnDescriptor));
        _actionReturnValue = actionReturnValue;
    }

    public object Wrap()
    {
        foreach (var wrapper in _actionResultWrappers)
        {
            if (wrapper.CanWrap(_actionReturnDescriptor))
            {
                return wrapper.Wrap(_response, _actionReturnDescriptor, _actionReturnValue);
            }
        }

        var actionReturnType = _actionReturnDescriptor.GetActionReturnType();

        throw new NotSupportedException(
            $"No ActionResult wrapper registered for '{actionReturnType.FullName}'. " +
            $"Status={(int)_response.StatusCode} ({_response.StatusCode}), " +
            $"ContentType={_response.Content.Headers.ContentType?.ToString() ?? "<null>"}.");
    }
}