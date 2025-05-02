using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing.RequestBodyContentParserStrategy;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing;

public sealed class RequestBodyContentParser : IRequestBodyContentParser
{
    private readonly IReadOnlyList<IRequestBodyContentParserStrategy> _strategies;

    public RequestBodyContentParser(IEnumerable<IRequestBodyContentParserStrategy> strategies)
    {
        _strategies = new List<IRequestBodyContentParserStrategy>(strategies);
    }

    public RequestBodyContent Parse(HttpRequest request, byte[] bodyBytes)
    {
        foreach (var s in _strategies)
        {
            if (s.CanParse(request))
            {
                return s.Parse(request, bodyBytes);
            }
        }

        return new RequestBodyContent
        {
            OriginalContentType = request.ContentType ?? string.Empty,
            ContentKind = RequestBodyContentKind.Unknown,
            ContentAsString = string.Empty
        };
    }
}