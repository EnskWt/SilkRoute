using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing;

public interface IRequestFormContentParser
{
    RequestFormContent Parse(HttpRequest request, IFormCollection form);
}