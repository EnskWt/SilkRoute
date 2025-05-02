using System.Collections;
using System.Reflection;

namespace SilkRoute.Internal.Extensions.Common;

internal static class ObjectExtensions
{
    public static bool ContainsType<T>(this object value, bool includeTopLevel = true) where T : class
    {
        if (value is null)
        {
            return false;
        }

        if (includeTopLevel && value is T)
        {
            return true;
        }

        if (value is IEnumerable<T>)
        {
            return true;
        }

        if (value is IEnumerable coll and not string)
        {
            foreach (var item in coll)
            {
                if (item is null)
                {
                    continue;
                }

                if (item.ContainsType<T>())
                {
                    return true;
                }
            }

            return false;
        }

        var type = value.GetType();

        if (type.IsSimpleScalarType())
        {
            return false;
        }

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            object pv;

            try
            {
                pv = prop.GetValue(value);
            }
            catch
            {
                continue;
            }

            if (pv.ContainsType<T>())
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsNonExplicitFormData(this object value)
    {
        return value.ContainsType<IFormFile>();
    }

    public static bool ContainsStream(this object value)
    {
        return value.ContainsType<Stream>();
    }

    public static bool ContainsNestedStream(this object value)
    {
        return value.ContainsType<Stream>(false);
    }

    public static bool ContainsByteArray(this object value)
    {
        return value.ContainsType<byte[]>();
    }
}