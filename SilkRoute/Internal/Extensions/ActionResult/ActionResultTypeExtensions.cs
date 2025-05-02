using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SilkRoute.Internal.Extensions.ActionResult;

internal static class ActionResultTypeExtensions
{
    public static bool IsActionResultLikeType(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return typeof(IActionResult).IsAssignableFrom(type) ||
               typeof(IConvertToActionResult).IsAssignableFrom(type);
    }

    public static bool IsAbstractActionResultType(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return type == typeof(IActionResult)
               || type == typeof(Microsoft.AspNetCore.Mvc.ActionResult)
               || type == typeof(IConvertToActionResult);
    }

    public static bool IsConcreteActionResultType(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!type.IsActionResultLikeType())
        {
            return false;
        }

        if (type.IsAbstractActionResultType())
        {
            return false;
        }

        if (type.IsGenericActionResultType())
        {
            return false;
        }

        if (type.IsInterface || type.IsAbstract)
        {
            return false;
        }

        return true;
    }

    public static bool IsGenericActionResultType(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return type.IsGenericType
               && type.GetGenericTypeDefinition() == typeof(ActionResult<>);
    }

    public static Type GetGenericActionResultValueType(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!type.IsGenericActionResultType() || !type.IsConstructedGenericType)
        {
            throw new InvalidOperationException(
                $"Type '{type}' is not a constructed ActionResult<T>.");
        }

        var args = type.GetGenericArguments();
        if (args.Length != 1)
        {
            throw new InvalidOperationException(
                $"Type '{type}' is expected to have exactly one generic argument.");
        }

        return args[0];
    }
}