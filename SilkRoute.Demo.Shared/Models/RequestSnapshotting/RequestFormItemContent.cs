using SilkRoute.Demo.Shared.Enums;

namespace SilkRoute.Demo.Shared.Models.RequestSnapshotting;

public sealed class RequestFormItemContent
{
    public string Name { get; set; }

    public RequestFormItemContentKind ContentKind { get; set; }

    public string ContentAsString { get; set; }
}