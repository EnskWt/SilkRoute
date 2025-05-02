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

    public bool ActionReturnTypeIndicatesVoid()
    {
        return _actionReturnType == typeof(void);
    }

    public bool ActionReturnTypeIndicatesString()
    {
        return _actionReturnType == typeof(string);
    }

    public bool ActionReturnTypeIndicatesStream()
    {
        return _actionReturnType == typeof(Stream);
    }

    public bool ActionReturnTypeIndicatesByteArray()
    {
        return _actionReturnType == typeof(byte[]);
    }

    public bool ActionReturnTypeIndicatesConcreteObject()
    {
        return !_actionReturnType.IsAbstract && !_actionReturnType.IsInterface;
    }
}