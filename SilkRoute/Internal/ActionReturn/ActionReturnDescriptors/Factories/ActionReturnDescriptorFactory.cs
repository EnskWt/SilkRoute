using SilkRoute.Internal.Abstractions.ActionReturn;
using SilkRoute.Internal.Extensions.ActionResult;

namespace SilkRoute.Internal.ActionReturn.ActionReturnDescriptors.Factories;

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