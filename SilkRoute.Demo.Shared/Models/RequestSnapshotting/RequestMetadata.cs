namespace SilkRoute.Demo.Shared.Models.RequestSnapshotting;

public sealed class RequestMetadata
{
    public DateTimeOffset TimestampUtc { get; set; }
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public string RoutePattern { get; set; }
}