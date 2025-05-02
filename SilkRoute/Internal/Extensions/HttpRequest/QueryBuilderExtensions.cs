using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;
using SilkRoute.Internal.Extensions.Common;
using SilkRoute.Internal.Serialization.Factories;

namespace SilkRoute.Internal.Extensions.HttpRequest;

internal static class QueryBuilderExtensions
{
    public static void AddFlattenedParameter(
        this QueryBuilder queryBuilder,
        string parameterName,
        object parameterValue)
    {
        if (queryBuilder is null)
        {
            throw new ArgumentNullException(nameof(queryBuilder));
        }

        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException("Parameter name cannot be null or whitespace.", nameof(parameterName));
        }

        if (parameterValue is null)
        {
            return;
        }

        ThrowIfContainsUnsupportedQueryData(parameterName, parameterValue);

        var token = JToken.FromObject(parameterValue, JsonSerializerFactory.CreateForQueryParameters());

        foreach (var (key, value) in FlattenQueryParameters(parameterName, token))
        {
            queryBuilder.Add(key, value);
        }
    }

    private static void ThrowIfContainsUnsupportedQueryData(string name, object value)
    {
        if (value.ContainsNonExplicitFormData())
        {
            throw new InvalidOperationException(
                $"Cannot bind form data in parameter '{name}' to query string. Use [FromForm] or remove form data parameter.");
        }

        if (value.ContainsStream())
        {
            throw new InvalidOperationException(
                $"Cannot bind stream data in parameter '{name}' to query string. Use [FromBody] or remove stream parameter.");
        }

        if (value.ContainsByteArray())
        {
            throw new InvalidOperationException(
                $"Cannot bind byte array data in parameter '{name}' to query string. Use [FromBody] or remove byte array parameter.");
        }
    }

    private static IEnumerable<(string Key, string Value)> FlattenQueryParameters(string prefix, JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
            {
                var obj = (JObject)token;

                foreach (var prop in obj.Properties())
                {
                    var childKey = $"{prefix}.{prop.Name}";

                    foreach (var kv in FlattenQueryParameters(childKey, prop.Value))
                    {
                        yield return kv;
                    }
                }

                break;
            }

            case JTokenType.Array:
            {
                var arr = (JArray)token;

                foreach (var item in arr)
                {
                    foreach (var kv in FlattenQueryParameters(prefix, item))
                    {
                        yield return kv;
                    }
                }

                break;
            }

            default:
            {
                yield return (prefix, token.ToString());
                break;
            }
        }
    }
}