using System.Text.RegularExpressions;

namespace SilkRoute.Tools.RequestTools.RequestHelpers
{
    internal static class RouteParameterHelper
    {
        private static readonly Regex PlaceholderPattern = new(@"\{(?<name>[^}:]+)(:(?<type>[^}]+))?\}", RegexOptions.Compiled);

        internal static string? ConvertToRouteTemplateType(Type t) =>
            t == typeof(int) ? "int" :
            t == typeof(long) ? "long" :
            t == typeof(Guid) ? "guid" :
            t == typeof(bool) ? "bool" :
            t == typeof(DateTime) ? "datetime" :
            null;

        internal static IReadOnlyList<(string Name, string? Type)> ExtractRouteParametersFromTemplate(string template) =>
            PlaceholderPattern.Matches(template)
                .Cast<Match>()
                .Select(m => (
                    Name: m.Groups["name"].Value,
                    Type: m.Groups["type"].Success ? m.Groups["type"].Value.ToLower() : null
                ))
                .ToList();
    }
}
