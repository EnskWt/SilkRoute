using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing.
    RequestFormItemContentParserStrategy;

public sealed class FileRequestFormItemContentParserStrategy : IRequestFormItemContentParserStrategy
{
    public bool CanParse(RequestFormItemContentParseContext ctx)
    {
        return ctx.File != null;
    }

    public RequestFormItemContent Parse(RequestFormItemContentParseContext ctx)
    {
        var f = ctx.File;

        using var input = f.OpenReadStream();
        using var ms = new MemoryStream();

        input.CopyTo(ms);

        var bytes = ms.ToArray();

        return new RequestFormItemContent
        {
            Name = ctx.Name,
            ContentKind = RequestFormItemContentKind.File,
            ContentAsString = bytes.Length != 0
                ? Convert.ToBase64String(bytes)
                : string.Empty
        };
    }
}