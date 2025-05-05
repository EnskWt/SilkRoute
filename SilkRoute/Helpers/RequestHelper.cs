using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SilkRoute.Helpers
{
    public static class RequestHelper
    {
        private static readonly Regex PlaceholderPattern = new(@"\{(?<name>[^}:]+)(:(?<type>[^}]+))?\}", RegexOptions.Compiled);

        public static bool IsPrimitive(Type t) =>
            t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid);

        public static string? MapTypeName(Type t) =>
            t == typeof(int) ? "int"
          : t == typeof(long) ? "long"
          : t == typeof(Guid) ? "guid"
          : t == typeof(bool) ? "bool"
          : t == typeof(DateTime) ? "datetime"
          : null;

        public static IReadOnlyList<(string Name, string? Type)> GetPlaceholders(string template) =>
            PlaceholderPattern.Matches(template)
                .Cast<Match>()
                .Select(m => (
                    Name: m.Groups["name"].Value,
                    Type: m.Groups["type"].Success ? m.Groups["type"].Value.ToLower() : null
                ))
                .ToList();
    }
}
