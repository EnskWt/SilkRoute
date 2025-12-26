using SilkRoute.Abstractions.Internal;

namespace SilkRoute.MethodResultDescriptors;

internal sealed class ActionResultMethodResultDescriptor : IMethodResultDescriptor
{
    private Type _resultType;
    private Type _actionResultType;
    private Type? _actionResultValueType;
    
    public ActionResultMethodResultDescriptor(
        Type resultType,
        Type actionResultType,
        Type? actionResultValueType)
    {
        _resultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
        _actionResultType = actionResultType ?? throw new ArgumentNullException(nameof(actionResultType));
        _actionResultValueType = actionResultValueType;
    }
}