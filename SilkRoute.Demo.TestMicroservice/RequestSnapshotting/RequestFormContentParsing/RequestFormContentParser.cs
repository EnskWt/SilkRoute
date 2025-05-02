using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing.
    RequestFormItemContentParserStrategy;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing;

public sealed class RequestFormContentParser : IRequestFormContentParser
{
    private readonly IReadOnlyList<IRequestFormItemContentParserStrategy> _strategies;

    public RequestFormContentParser(IEnumerable<IRequestFormItemContentParserStrategy> strategies)
    {
        _strategies = new List<IRequestFormItemContentParserStrategy>(strategies);
    }

    public RequestFormContent Parse(HttpRequest request, IFormCollection form)
    {
        var items = new List<RequestFormItemContent>();

        foreach (var kv in form)
        {
            var ctx = new RequestFormItemContentParseContext
            {
                Name = kv.Key,
                Values = kv.Value.ToArray(),
                File = null
            };

            items.Add(ParseItemContent(ctx));
        }

        foreach (var f in form.Files)
        {
            var ctx = new RequestFormItemContentParseContext
            {
                Name = f.Name,
                Values = null,
                File = f
            };

            items.Add(ParseItemContent(ctx));
        }

        return new RequestFormContent
        {
            OriginalContentType = request.ContentType ?? string.Empty,
            Items = items.ToArray()
        };
    }

    private RequestFormItemContent ParseItemContent(RequestFormItemContentParseContext ctx)
    {
        foreach (var s in _strategies)
        {
            if (s.CanParse(ctx))
            {
                return s.Parse(ctx);
            }
        }

        return new RequestFormItemContent
        {
            Name = ctx.Name ?? string.Empty,
            ContentKind = RequestFormItemContentKind.Unknown,
            ContentAsString = string.Empty
        };
    }
}