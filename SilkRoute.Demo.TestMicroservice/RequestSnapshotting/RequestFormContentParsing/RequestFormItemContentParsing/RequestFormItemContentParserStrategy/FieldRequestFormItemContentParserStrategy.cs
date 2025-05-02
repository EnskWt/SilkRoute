using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing.
    RequestFormItemContentParserStrategy;

public sealed class FieldRequestFormItemContentParserStrategy : IRequestFormItemContentParserStrategy
{
    public bool CanParse(RequestFormItemContentParseContext ctx)
    {
        return ctx.Values != null;
    }

    public RequestFormItemContent Parse(RequestFormItemContentParseContext ctx)
    {
        var text = ctx.Values.Length == 0 ? string.Empty : string.Join(", ", ctx.Values);

        return new RequestFormItemContent
        {
            Name = ctx.Name,
            ContentKind = RequestFormItemContentKind.Field,
            ContentAsString = text
        };
    }
}