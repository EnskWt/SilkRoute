using System.Text.RegularExpressions;
using SilkRoute.Internal.Constants;

namespace SilkRoute.Internal.Extensions.HttpRequest;

internal static class RouteTemplateExtensions
{
    private static readonly Regex PlaceholderPattern =
        new(@"\{(?<name>[^}:]+)(:(?<constraints>[^}]+))?\}", RegexOptions.Compiled);

    public static IReadOnlyList<(string Name, string? TypeConstraint)> ExtractRouteParameters(this string template)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        return PlaceholderPattern.Matches(template)
            .Select(m =>
            {
                var name = m.Groups["name"].Value;

                string? typeConstraint = null;
                if (m.Groups["constraints"].Success)
                {
                    typeConstraint = ExtractTypeConstraint(m.Groups["constraints"].Value);
                }

                return (Name: name, TypeConstraint: typeConstraint);
            })
            .ToList();
    }

    private static string? ExtractTypeConstraint(string constraints)
    {
        foreach (var part in constraints.Split(':', StringSplitOptions.RemoveEmptyEntries))
        {
            var constraint = part;
            var paren = constraint.IndexOf('(');
            if (paren >= 0)
            {
                constraint = constraint.Substring(0, paren);
            }

            if (RouteConstraintConstants.TypeConstraints.Contains(constraint))
            {
                return constraint.ToLowerInvariant();
            }
        }

        return null;
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

        var result = PlaceholderPattern.Replace(template, m =>
        {
            var name = m.Groups["name"].Value;
            var constraintsText = m.Groups["constraints"].Success ? m.Groups["constraints"].Value : null;

            if (!routeValues.TryGetValue(name, out var value))
            {
                return m.Value;
            }

            if (!RouteConstraintConstants.MatchesAll(constraintsText, value))
            {
                return m.Value;
            }

            return Uri.EscapeDataString(value);
        });

        if (PlaceholderPattern.IsMatch(result))
        {
            var missingOrMismatched = result.ExtractRouteParameters()
                .Select(x => x.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            throw new InvalidOperationException(
                $"Not all route parameters were provided or matched constraints. Missing: {string.Join(", ", missingOrMismatched)}. Template: '{template}'.");
        }

        return result;
    }
}