using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing.
    RequestFormItemContentParserStrategy;

public interface IRequestFormItemContentParserStrategy
{
    bool CanParse(RequestFormItemContentParseContext ctx);

    RequestFormItemContent Parse(RequestFormItemContentParseContext ctx);
}