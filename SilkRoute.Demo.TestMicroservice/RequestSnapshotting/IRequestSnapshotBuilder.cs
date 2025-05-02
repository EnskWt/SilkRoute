using SilkRoute.Demo.Shared.Models.RequestSnapshotting;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting;

public interface IRequestSnapshotBuilder
{
    Task<RequestSnapshot> BuildAsync(HttpContext httpContext, CancellationToken ct);
}