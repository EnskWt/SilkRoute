using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Abstractions.HttpResponse;
using SilkRoute.Internal.Extensions.ActionResult;
using SilkRoute.Internal.Extensions.HttpResponse;

namespace SilkRoute.Internal.HttpResponse.HttpResponseContentReaders;

internal sealed class JsonContentReader : IHttpResponseContentReader
{
    public int Priority => int.MaxValue;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeIndicatesConcreteObject() || descriptor.GetActionReturnType().IsAbstractActionResultType())
        {
            return responseMessage.IsJsonMediaType();
        }

        return false;
    }

    public async Task<object> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor)
    {
        var json = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        var actionReturnType = descriptor.GetActionReturnType();

        var targetType = actionReturnType;

        if (actionReturnType.IsGenericActionResultType())
        {
            targetType = actionReturnType.GetGenericActionResultValueType();
        }

        if (actionReturnType.IsAbstractActionResultType() || actionReturnType.IsConcreteActionResultType())
        {
            targetType = typeof(ExpandoObject);
        }

        JsonSerializerSettings settings = null;
        if (targetType == typeof(ExpandoObject))
        {
            settings = new JsonSerializerSettings
            {
                Converters = { new ExpandoObjectConverter() }
            };
        }

        return settings is null
            ? JsonConvert.DeserializeObject(json, targetType)
            : JsonConvert.DeserializeObject(json, targetType, settings);
    }
}