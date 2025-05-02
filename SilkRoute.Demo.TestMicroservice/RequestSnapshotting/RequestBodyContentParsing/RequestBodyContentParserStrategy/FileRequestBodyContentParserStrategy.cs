using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Extensions;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing.
    RequestBodyContentParserStrategy;

public sealed class FileRequestBodyContentParserStrategy : IRequestBodyContentParserStrategy
{
    public bool CanParse(HttpRequest request)
    {
        return request.IsFileMediaType();
    }

    public RequestBodyContent Parse(HttpRequest request, byte[] bodyBytes)
    {
        return new RequestBodyContent
        {
            OriginalContentType = request.ContentType ?? string.Empty,
            ContentKind = RequestBodyContentKind.File,
            ContentAsString = bodyBytes != null && bodyBytes.Length != 0
                ? Convert.ToBase64String(bodyBytes)
                : string.Empty
        };
    }
}