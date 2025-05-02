using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SilkRoute.Demo.Shared.Enums;
using SilkRoute.Demo.Shared.Extensions;
using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing.
    RequestBodyContentParserStrategy;

public sealed class JsonRequestBodyContentParserStrategy : IRequestBodyContentParserStrategy
{
    public bool CanParse(HttpRequest request)
    {
        return request.IsJsonMediaType();
    }

    public RequestBodyContent Parse(HttpRequest request, byte[] bodyBytes)
    {
        var text = bodyBytes.DecodeUtf8();
        var pretty = FormatJson(text);

        return new RequestBodyContent
        {
            OriginalContentType = request.ContentType ?? string.Empty,
            ContentKind = RequestBodyContentKind.Json,
            ContentAsString = pretty
        };
    }

    private static string FormatJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var token = JToken.Parse(text);
        return token.ToString(Formatting.Indented);
    }
}