using SilkRoute.Abstractions.Internal;

namespace SilkRoute.MethodResultDescriptors.MethodResultDescriptors;

internal sealed class DirectMethodResultDescriptor : IMethodResultDescriptor
{
    public DirectMethodResultDescriptor(Type resultType)
    {
        ResultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
    }

    public Type ResultType { get; }
}