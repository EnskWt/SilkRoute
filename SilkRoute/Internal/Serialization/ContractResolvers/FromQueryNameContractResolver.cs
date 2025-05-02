using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SilkRoute.Internal.Extensions.ModelBinding;

namespace SilkRoute.Internal.Serialization.ContractResolvers;

internal sealed class FromQueryNameContractResolver : DefaultContractResolver
{
    public static readonly FromQueryNameContractResolver Instance = new();
    
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);
        
        if (member is PropertyInfo pi)
        {
            prop.PropertyName = pi.GetModelBindingNameOrDefault<FromQueryAttribute>();
        }

        return prop;
    }
}