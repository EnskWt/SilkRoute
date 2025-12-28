using SilkRoute.Internal.Constants;

namespace SilkRoute.Internal.Extensions.HttpRequest;

internal static class RouteConstraintTypeExtensions
{
    public static string? ToRouteTypeConstraint(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        type = Nullable.GetUnderlyingType(type) ?? type;

        return RouteConstraintConstants.TypeToConstraint.GetValueOrDefault(type);
    }
}