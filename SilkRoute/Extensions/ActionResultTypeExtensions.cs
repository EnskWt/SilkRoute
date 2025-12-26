using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SilkRoute.Tools.MvcTools.Extensions;

// TODO: maybe move to other folder
internal static class ActionResultTypeExtensions
{
    public static bool IsActionResultLike(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return typeof(IActionResult).IsAssignableFrom(type) ||
               typeof(IConvertToActionResult).IsAssignableFrom(type);
    }
     
    public static bool TryGetActionResultValueType(this Type type, out Type valueType)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
        {
            valueType = type.GetGenericArguments()[0];
            return true;
        }

        valueType = null!;
        return false;
    }
}