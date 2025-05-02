namespace SilkRoute.Demo.Shared.Models.RequestSnapshotting;

public sealed class RequestRawData
{
    public Dictionary<string, string> Route { get; set; }
    public Dictionary<string, string> Query { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public RequestBodyContent Body { get; set; }
    public RequestFormContent Form { get; set; }
}