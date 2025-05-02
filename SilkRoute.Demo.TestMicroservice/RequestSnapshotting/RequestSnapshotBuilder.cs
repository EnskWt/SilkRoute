using SilkRoute.Demo.Shared.Models.RequestSnapshotting;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing;

namespace SilkRoute.Demo.TestMicroservice.RequestSnapshotting;

public sealed class RequestSnapshotBuilder : IRequestSnapshotBuilder
{
    private readonly IRequestBodyContentParser _requestBodyContentParser;
    private readonly IRequestFormContentParser _requestFormContentParser;

    public RequestSnapshotBuilder(IRequestBodyContentParser requestBodyContentParser,
        IRequestFormContentParser requestFormContentParser)
    {
        _requestBodyContentParser = requestBodyContentParser;
        _requestFormContentParser = requestFormContentParser;
    }

    public async Task<RequestSnapshot> BuildAsync(HttpContext httpContext, CancellationToken ct)
    {
        var snapshot = new RequestSnapshot
        {
            Metadata = BuildRequestMetadata(httpContext),
            RawData = await BuildRequestRawData(httpContext, ct)
        };

        return snapshot;
    }

    private RequestMetadata BuildRequestMetadata(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint() as RouteEndpoint;

        return new RequestMetadata
        {
            TimestampUtc = DateTimeOffset.UtcNow,
            HttpMethod = httpContext.Request.Method,
            Path = $"{httpContext.Request.Path}{httpContext.Request.QueryString}",
            RoutePattern = endpoint?.RoutePattern.RawText ?? string.Empty
        };
    }

    private async Task<RequestRawData> BuildRequestRawData(HttpContext httpContext, CancellationToken ct)
    {
        var route = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in httpContext.Request.RouteValues)
        {
            route[kv.Key] = kv.Value != null ? kv.Value.ToString() : string.Empty;
        }

        var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in httpContext.Request.Query)
        {
            query[kv.Key] = kv.Value.ToString();
        }

        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in httpContext.Request.Headers)
        {
            headers[kv.Key] = kv.Value.ToString();
        }

        var body = await BuildRequestBodyContent(httpContext, ct);
        var form = await BuildRequestFormContent(httpContext, ct);

        return new RequestRawData
        {
            Route = route,
            Query = query,
            Headers = headers,
            Body = body,
            Form = form
        };
    }

    private async Task<RequestBodyContent> BuildRequestBodyContent(HttpContext httpContext, CancellationToken ct)
    {
        var request = httpContext.Request;

        if (request.HasFormContentType)
        {
            return null;
        }

        if (request.Body == Stream.Null)
        {
            return null;
        }

        if (request.Body.CanSeek)
        {
            request.Body.Position = 0;
        }

        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            await request.Body.CopyToAsync(ms, ct);
            bytes = ms.ToArray();
        }

        if (request.Body.CanSeek)
        {
            request.Body.Position = 0;
        }

        if (bytes.Length == 0)
        {
            return null;
        }

        return _requestBodyContentParser.Parse(request, bytes);
    }

    private async Task<RequestFormContent> BuildRequestFormContent(HttpContext httpContext, CancellationToken ct)
    {
        var request = httpContext.Request;

        if (!request.HasFormContentType)
        {
            return null;
        }

        var form = await request.ReadFormAsync(ct);

        if (form.Count == 0 && form.Files.Count == 0)
        {
            return null;
        }

        return _requestFormContentParser.Parse(request, form);
    }
}