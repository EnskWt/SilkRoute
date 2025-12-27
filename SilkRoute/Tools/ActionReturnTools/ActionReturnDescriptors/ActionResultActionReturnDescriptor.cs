using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors;

internal sealed class ActionResultActionReturnDescriptor : IActionReturnDescriptor
{
    private readonly Type _actionResultActionReturnType;

    public ActionResultActionReturnDescriptor(Type actionResultActionReturnType)
    {
        _actionResultActionReturnType = actionResultActionReturnType
            ?? throw new ArgumentNullException(nameof(actionResultActionReturnType));
    }

    public Type GetActionReturnType()
    {
        return _actionResultActionReturnType;
    }

    public bool ActionReturnTypeMatchesVoid()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(void);
        }

        return false;
    }

    public bool ActionReturnTypeMatchesString()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(string);
        }

        return typeof(ContentResult).IsAssignableFrom(_actionResultActionReturnType);
    }

    public bool ActionReturnTypeMatchesStream()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(Stream);
        }

        return typeof(FileStreamResult).IsAssignableFrom(_actionResultActionReturnType);
    }

    public bool ActionReturnTypeMatchesByteArray()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(byte[]);
        }

        return typeof(FileContentResult).IsAssignableFrom(_actionResultActionReturnType);
    }

    public bool ActionReturnTypeMatchesJson()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            var valueType = _actionResultActionReturnType.GetGenericActionResultValueType();

            if (valueType == typeof(void))
            {
                return false;
            }

            if (valueType == typeof(string))
            {
                return false;
            }

            if (valueType == typeof(Stream))
            {
                return false;
            }

            if (valueType == typeof(byte[]))
            {
                return false;
            }

            return true;
        }

        if (typeof(JsonResult).IsAssignableFrom(_actionResultActionReturnType))
        {
            return true;
        }
        
        if (typeof(ObjectResult).IsAssignableFrom(_actionResultActionReturnType))
        {
            return true;
        }

        return false;
    }
}