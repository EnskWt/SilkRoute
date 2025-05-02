namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing;

public class RequestFormItemContentParseContext
{
    public string Name { get; set; }

    public string[] Values { get; set; }
    public IFormFile File { get; set; }
}