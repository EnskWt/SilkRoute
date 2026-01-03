using SilkRoute.Internal.Abstractions.ActionReturn;

namespace SilkRoute.Internal.ActionReturn.ActionReturnDescriptors;

internal sealed class DirectActionReturnDescriptor : IActionReturnDescriptor
{
    private readonly Type _actionReturnType;

    public DirectActionReturnDescriptor(Type actionReturnType)
    {
        _actionReturnType = actionReturnType ?? throw new ArgumentNullException(nameof(actionReturnType));
    }
    
    public Type GetActionReturnType()
    {
        return _actionReturnType;
    }

    public bool ActionReturnTypeMatchesVoid()
    {
        return _actionReturnType == typeof(void);
    }

    public bool ActionReturnTypeMatchesString()
    {
        return _actionReturnType == typeof(string);
    }

    public bool ActionReturnTypeMatchesStream()
    {
        return _actionReturnType == typeof(Stream);
    }

    public bool ActionReturnTypeMatchesByteArray()
    {
        return _actionReturnType == typeof(byte[]);
    }
    
    public bool ActionReturnTypeIsAbstractOrInterface()
    {
        return _actionReturnType.IsAbstract || _actionReturnType.IsInterface;
    }
}
