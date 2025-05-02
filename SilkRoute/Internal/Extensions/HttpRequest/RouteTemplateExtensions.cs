using System.Text.RegularExpressions;

namespace SilkRoute.Internal.Extensions.HttpRequest;

internal static class RouteTemplateExtensions
{
    private static readonly Regex RouteParameterPattern =
        new(@"\{\*?(?<name>[^}:?=]+)(?:\?(?=[:}])|=[^}:]+)?(?:\:(?<constraints>[^}]+))?\}",
            RegexOptions.Compiled);

    public static IReadOnlyList<string> ExtractRouteParameters(this string template)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        return RouteParameterPattern.Matches(template)
            .Select(m => m.Groups["name"].Value)
            .ToList();
    }

    public static string ApplyRouteValues(this string template, IReadOnlyDictionary<string, string> routeValues)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        if (routeValues is null)
        {
            throw new ArgumentNullException(nameof(routeValues));
        }

        if (string.IsNullOrWhiteSpace(template) || routeValues.Count == 0)
        {
            return template;
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in routeValues)
        {
            if (!values.ContainsKey(kv.Key))
            {
                values.Add(kv.Key, kv.Value);
            }
        }

        var result = RouteParameterPattern.Replace(template, m =>
        {
            var name = m.Groups["name"].Value;

            if (!values.TryGetValue(name, out var value))
            {
                return m.Value;
            }

            return Uri.EscapeDataString(value);
        });

        if (RouteParameterPattern.IsMatch(result))
        {
            var missing = result.ExtractRouteParameters()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            throw new InvalidOperationException(
                $"Not all route parameters were provided. Missing: {string.Join(", ", missing)}. Template: '{template}'.");
        }

        return result;
    }
}