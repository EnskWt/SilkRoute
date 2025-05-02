using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Extensions.ActionResult;

namespace SilkRoute.Internal.ActionReturn.ActionReturnDescriptors;

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

    public bool ActionReturnTypeIndicatesVoid()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(void);
        }

        return false;
    }

    public bool ActionReturnTypeIndicatesString()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(string);
        }

        return typeof(ContentResult).IsAssignableFrom(_actionResultActionReturnType);
    }

    public bool ActionReturnTypeIndicatesStream()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(Stream);
        }

        return typeof(FileStreamResult).IsAssignableFrom(_actionResultActionReturnType);
    }

    public bool ActionReturnTypeIndicatesByteArray()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            return _actionResultActionReturnType.GetGenericActionResultValueType() == typeof(byte[]);
        }

        return typeof(FileContentResult).IsAssignableFrom(_actionResultActionReturnType);
    }

    public bool ActionReturnTypeIndicatesConcreteObject()
    {
        if (_actionResultActionReturnType.IsGenericActionResultType())
        {
            var type = _actionResultActionReturnType.GetGenericActionResultValueType();
            return !type.IsAbstract && !type.IsInterface;
        }

        return typeof(ObjectResult).IsAssignableFrom(_actionResultActionReturnType) 
               || typeof(JsonResult).IsAssignableFrom(_actionResultActionReturnType);
    }
}