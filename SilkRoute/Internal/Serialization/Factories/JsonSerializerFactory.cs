using Newtonsoft.Json;
using SilkRoute.Internal.Serialization.ContractResolvers;

namespace SilkRoute.Internal.Serialization.Factories;

public static class JsonSerializerFactory
{
    public static JsonSerializer CreateForQueryParameters()
    {
        var serializer = JsonSerializer.CreateDefault();
        serializer.ContractResolver = FromQueryNameContractResolver.Instance;
        return serializer;
    }
}