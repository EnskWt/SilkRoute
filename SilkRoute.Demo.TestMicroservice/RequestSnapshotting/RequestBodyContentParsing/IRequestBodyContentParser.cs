using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing;

public interface IRequestBodyContentParser
{
    RequestBodyContent Parse(HttpRequest request, byte[] bodyBytes);
}