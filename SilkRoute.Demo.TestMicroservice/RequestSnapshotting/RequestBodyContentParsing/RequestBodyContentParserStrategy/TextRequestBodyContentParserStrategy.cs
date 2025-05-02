using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Extensions;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing.
    RequestBodyContentParserStrategy;

public sealed class TextRequestBodyContentParserStrategy : IRequestBodyContentParserStrategy
{
    public bool CanParse(HttpRequest request)
    {
        return request.IsTextLikeMediaType();
    }

    public RequestBodyContent Parse(HttpRequest request, byte[] bodyBytes)
    {
        return new RequestBodyContent
        {
            OriginalContentType = request.ContentType ?? string.Empty,
            ContentKind = RequestBodyContentKind.Text,
            ContentAsString = bodyBytes.DecodeUtf8()
        };
    }
}