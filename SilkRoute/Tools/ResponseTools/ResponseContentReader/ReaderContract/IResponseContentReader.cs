using SilkRoute.Tools.ActionReturnTools.ActionReturnDescriptors.ActionReturnDescriptorContract;

namespace SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

internal interface IResponseContentReader
{
    int Priority { get; }

    bool CanRead(HttpResponseMessage responseMessage, IActionReturnDescriptor descriptor);

    Task<object?> ReadAsync(
        HttpResponseMessage response,
        IActionReturnDescriptor descriptor);
}