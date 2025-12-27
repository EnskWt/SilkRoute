using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper;

internal sealed class StatusCodeResultWrapper : IActionResultWrapper
{
    public int Priority => 40;

    public bool CanWrap(IActionReturnDescriptor actionReturnDescriptor)
    {
        if (actionReturnDescriptor is null)
        {
            throw new ArgumentNullException(nameof(actionReturnDescriptor));
        }

        var returnType = actionReturnDescriptor.GetActionReturnType();
        return typeof(StatusCodeResult).IsAssignableFrom(returnType);
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

        var actionReturnType = actionReturnDescriptor.GetActionReturnType();
        var statusCode = (int)response.StatusCode;

        var ctorInt = actionReturnType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(c =>
            {
                var ps = c.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(int);
            });

        if (ctorInt != null)
        {
            return (StatusCodeResult)ctorInt.Invoke(new object[] { statusCode });
        }

        var ctorEmpty = actionReturnType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(c => c.GetParameters().Length == 0);

        if (ctorEmpty != null)
        {
            return (StatusCodeResult)Activator.CreateInstance(actionReturnType)!;
        }

        return (StatusCodeResult)Activator.CreateInstance(typeof(StatusCodeResult), statusCode)!;
    }
}