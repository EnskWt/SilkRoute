using System.Reflection;
using SilkRoute.Internal.Abstractions.ActionResult;
using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Extensions.ActionResult;

namespace SilkRoute.Internal.ActionResult.ActionResultWrappers;

internal sealed class GenericActionResultWrapper : IActionResultWrapper
{
    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType.IsConstructedGenericType && actionReturnType.IsGenericActionResultType();
    }

    public object Wrap(
        HttpResponseMessage response,
        IActionReturnDescriptor actionReturnDescriptor,
        object actionReturnValue)
    {
        var actionResultActionReturnType = actionReturnDescriptor.GetActionReturnType();
        var valueType = actionResultActionReturnType.GetGenericActionResultValueType();

        var effectiveValue = actionReturnValue;
        if (effectiveValue is null && valueType.IsValueType)
        {
            effectiveValue = Activator.CreateInstance(valueType);
        }

        var ctor = actionResultActionReturnType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(ci =>
            {
                var ps = ci.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == valueType;
            });

        if (ctor is not null)
        {
            return ctor.Invoke(new[] { effectiveValue! });
        }

        throw new InvalidOperationException(
            $"No suitable ctor or implicit conversion found for '{actionResultActionReturnType.FullName}' from '{valueType.FullName}'.");
    }
}