using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing.
    RequestBodyContentParserStrategy;

public interface IRequestBodyContentParserStrategy
{
    bool CanParse(HttpRequest request);

    RequestBodyContent Parse(HttpRequest request, byte[] bodyBytes);
}