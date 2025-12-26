namespace SilkRoute.Tools.ResponseTools.ResponseContentReader.ReaderContract;

internal interface IResponseContentReader
{
    int Priority { get; }

    bool CanRead(
        Type responseType,
        Type payloadType,
        bool isActionResult,
        HttpResponseMessage response);

    Task<object?> ReadAsync(
        HttpResponseMessage response,
        Type responseType,
        Type payloadType,
        bool isActionResult,
        CancellationToken cancellationToken = default);
}