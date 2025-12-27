namespace SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

internal interface IActionReturnDescriptor
{
    Type GetActionReturnType();
    
    bool ActionReturnTypeMatchesVoid();
    bool ActionReturnTypeMatchesString();
    bool ActionReturnTypeMatchesStream();
    bool ActionReturnTypeMatchesByteArray();
    bool ActionReturnTypeMatchesJson();
}