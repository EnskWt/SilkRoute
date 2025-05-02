namespace SilkRoute.Demo.Shared.Models.RequestSnapshotting;

public sealed class RequestFormContent
{
    public string OriginalContentType { get; set; }

    public RequestFormItemContent[] Items { get; set; }
}