namespace SilkRoute.Internal.Abstractions.ActionReturn;

internal interface IActionReturnDescriptor
{
    Type GetActionReturnType();
    
    bool ActionReturnTypeMatchesVoid();
    bool ActionReturnTypeMatchesString();
    bool ActionReturnTypeMatchesStream();
    bool ActionReturnTypeMatchesByteArray();
    bool ActionReturnTypeIsAbstractOrInterface();
}