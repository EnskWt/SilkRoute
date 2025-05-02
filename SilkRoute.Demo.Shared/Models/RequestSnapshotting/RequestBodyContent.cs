using SilkRoute.Demo.Shared.Enums;

namespace SilkRoute.Demo.Shared.Models.RequestSnapshotting;

public sealed class RequestBodyContent
{
    public string OriginalContentType { get; set; }
    public RequestBodyContentKind ContentKind { get; set; }
    public string ContentAsString { get; set; }
}