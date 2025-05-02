namespace SilkRoute.Internal.Abstractions.ActionReturn;

internal interface IActionReturnDescriptor
{
    Type GetActionReturnType();

    bool ActionReturnTypeIndicatesVoid();
    bool ActionReturnTypeIndicatesString();
    bool ActionReturnTypeIndicatesStream();
    bool ActionReturnTypeIndicatesByteArray();
    bool ActionReturnTypeIndicatesConcreteObject();
}