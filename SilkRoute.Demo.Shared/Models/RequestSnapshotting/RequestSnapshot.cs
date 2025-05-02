namespace SilkRoute.Demo.Shared.Models.RequestSnapshotting;

public sealed class RequestSnapshot
{
    public RequestMetadata Metadata { get; set; }
    public RequestRawData RawData { get; set; }
}