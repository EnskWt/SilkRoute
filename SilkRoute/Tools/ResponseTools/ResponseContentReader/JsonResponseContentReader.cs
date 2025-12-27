using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;
using SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;
using SilkRoute.Tools.ResponseTools.ResponseExtensions;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader;

internal sealed class JsonResponseContentReader : IResponseContentReader
{
    public int Priority => int.MaxValue;

    public bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor)
    {
        if (descriptor.ActionReturnTypeMatchesJson())
        {
            return true;
        }

        if (descriptor.GetActionReturnType().IsAbstractActionResultType())
        {
            if (responseMessage.IsJsonMediaType())
            {
                return true;
            }
        }

        return false;
    }

    public async Task<object?> ReadAsync(
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

        JsonSerializerSettings? settings = null;
        if (targetType == typeof(ExpandoObject))
        {
            settings = new JsonSerializerSettings
            {
                Converters = { new ExpandoObjectConverter() }
            };
        }

        return settings == null
            ? JsonConvert.DeserializeObject(json, targetType)
            : JsonConvert.DeserializeObject(json, targetType, settings);
    }
}