namespace SilkRoute.Internal.Extensions.Common;

internal static class TypeExtensions
{
    public static bool IsSimpleScalarType(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(Guid)
               || type == typeof(TimeSpan);
    }
}