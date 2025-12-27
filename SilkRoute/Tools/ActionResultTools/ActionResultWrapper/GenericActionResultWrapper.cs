using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper;

internal sealed class GenericActionResultWrapper : IActionResultWrapper
{
    public int Priority => 0;

    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        return actionReturnType.IsConstructedGenericType && actionReturnType.IsGenericActionResultType();
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

        var actionResultActionReturnType = actionReturnDescriptor.GetActionReturnType();
        var valueType = actionResultActionReturnType.GetGenericActionResultValueType();

        var effectiveValue = actionReturnValue;
        if (effectiveValue == null && valueType.IsValueType)
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

        if (ctor != null)
        {
            return ctor.Invoke(new[] { effectiveValue! });
        }

        throw new InvalidOperationException(
            $"No suitable ctor or implicit conversion found for '{actionResultActionReturnType.FullName}' from '{valueType.FullName}'.");
    }
}