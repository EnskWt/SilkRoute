using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SilkRoute.Internal.Extensions.ModelBinding;

internal static class ModelBindingNameExtensions
{
    public static string GetModelBindingNameOrDefault<TAttribute>(this ParameterInfo parameterInfo)
        where TAttribute : Attribute, IModelNameProvider
    {
        var attr = parameterInfo.GetCustomAttribute<TAttribute>(true);

        if (attr is not null && !string.IsNullOrWhiteSpace(attr.Name))
        {
            return attr.Name!;
        }

        return parameterInfo.Name!;
    }

    public static string GetModelBindingNameOrDefault<TAttribute>(this PropertyInfo propertyInfo)
        where TAttribute : Attribute, IModelNameProvider
    {
        var attr = propertyInfo.GetCustomAttribute<TAttribute>(true);

        if (attr is not null && !string.IsNullOrWhiteSpace(attr.Name))
        {
            return attr.Name!;
        }

        return propertyInfo.Name;
    }
}