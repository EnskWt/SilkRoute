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

        internal static string ApplyRouteParameters(string template, IReadOnlyDictionary<string, string> routeValues)
        {
            if (string.IsNullOrWhiteSpace(template) || routeValues.Count == 0)
                return template;

            var result = PlaceholderPattern.Replace(template, m =>
            {
                var name = m.Groups["name"].Value;

                if (routeValues.TryGetValue(name, out var value) && value != null)
                    return Uri.EscapeDataString(value);

                return m.Value;
            });

            if (PlaceholderPattern.IsMatch(result))
            {
                var missing = ExtractRouteParametersFromTemplate(result)
                    .Select(x => x.Name)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                throw new InvalidOperationException(
                    $"Not all route parameters were provided. Missing: {string.Join(", ", missing)}. Template: '{template}'.");
            }

            return result;
        }
    }
}
