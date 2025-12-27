using SilkRoute.Tools.ActionResultTools.ActionResultExtensions;
using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorFactory;

internal static class ActionReturnDescriptorFactory
{
    public static IActionReturnDescriptor Create(Type actionReturnType)
    {
        if (actionReturnType is null)
        {
            throw new ArgumentNullException(nameof(actionReturnType));
        }

        if (!actionReturnType.IsActionResultLikeType())
        {
            return new DirectActionReturnDescriptor(actionReturnType);
        }

        return new ActionResultActionReturnDescriptor(actionReturnType);
    }
}