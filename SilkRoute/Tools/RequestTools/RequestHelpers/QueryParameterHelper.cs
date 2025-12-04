using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;

namespace SilkRoute.Tools.RequestTools.RequestHelpers
{
    internal static class QueryParameterHelper
    {
        internal static void AddQueryParams(QueryBuilder queryBuilder, string parameterName, object parameterValue)
        {
            if (parameterValue == null)
            {
                return;
            }

            ThrowIfContainsFormData(parameterName, parameterValue);
            ThrowIfContainsStream(parameterName, parameterValue);

            var token = JToken.FromObject(parameterValue);

            foreach (var (key, val) in ExtractQueryParameters(parameterName, token))
            {
                queryBuilder.Add(key, val);
            }
        }

        private static IEnumerable<(string Key, string Value)> ExtractQueryParameters(string prefix, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var obj = (JObject)token;
                    foreach (var prop in obj.Properties())
                    {
                        string childKey = $"{prefix}.{prop.Name}";
                        foreach (var kv in ExtractQueryParameters(childKey, prop.Value))
                        {
                            yield return kv;
                        }
                    }
                    break;

                case JTokenType.Array:
                    var arr = (JArray)token;
                    foreach (var item in arr)
                    {
                        foreach (var kv in ExtractQueryParameters(prefix, item))
                        {
                            yield return kv;
                        }
                    }
                    break;

                default:
                    yield return (prefix, token.ToString());
                    break;
            }
        }

        private static void ThrowIfContainsFormData(string name, object value)
        {
            if (RequestTypeHelper.ContainsNonExplicitFormData(value))
                throw new InvalidOperationException(
                    $"Cannot bind form data in parameter '{name}' to query string. Use [FromForm] or remove form data parameter.");
        }

        private static void ThrowIfContainsStream(string name, object value)
        {
            if (RequestTypeHelper.ContainsStream(value))
                throw new InvalidOperationException(
                    $"Cannot bind stream data in parameter '{name}' to query string. Use [FromBody] or remove stream parameter.");
        }
    }
}
