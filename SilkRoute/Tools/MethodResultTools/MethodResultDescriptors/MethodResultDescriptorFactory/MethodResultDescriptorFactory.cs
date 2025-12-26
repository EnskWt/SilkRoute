using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SilkRoute.Abstractions.Internal;
using SilkRoute.MethodResultDescriptors;
using SilkRoute.MethodResultDescriptors.MethodResultDescriptors;
using SilkRoute.Tools.MvcTools.Extensions;

namespace SilkRoute.Tools.MethodResultTools.MethodResultDescriptors.MethodResultDescriptorFactory;

internal static class MethodResultDescriptorFactory
{
    public static IMethodResultDescriptor Create(Type resultType)
    {
        if (resultType == null)
        {
            throw new ArgumentNullException(nameof(resultType));
        }

        if (!resultType.IsActionResultLike())
        {
            return new DirectMethodResultDescriptor(resultType);
        }

        Type? valueType = null;
        if (resultType.TryGetActionResultValueType(out var vt))
        {
            valueType = vt;
        }

        return new ActionResultMethodResultDescriptor(resultType, resultType, valueType);
    }
}