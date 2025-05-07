using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SilkRoute.Tools.RequestTools.RequestHelpers
{
    internal static class RequestTypeHelper
    {
        internal static bool IsPrimitive(Type t) =>
            t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid);

        internal static bool ContainsType<T>(object? val, bool includeTopLevel) where T : class
        {
            if (val == null) return false;
            if (includeTopLevel && val is T) return true;
            if (val is IEnumerable<T>) return true;
            if (val is IEnumerable coll && val is not string)
            {
                foreach (var item in coll)
                    if (ContainsType<T>(item, includeTopLevel: true)) return true;
                return false;
            }

            var t = val.GetType();
            if (IsPrimitive(t)) return false;

            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetIndexParameters().Length > 0) continue;
                object? pv;
                try { pv = prop.GetValue(val); }
                catch { continue; }
                if (ContainsType<T>(pv, includeTopLevel: true)) return true;
            }

            return false;
        }

        internal static bool ContainsNonExplicitFormData(object? val) => ContainsType<IFormFile>(val, includeTopLevel: true);

        internal static bool ContainsStream(object? val) => ContainsType<Stream>(val, includeTopLevel: true);

        internal static bool ContainsNestedStream(object? val) => ContainsType<Stream>(val, includeTopLevel: false);
    }
}
