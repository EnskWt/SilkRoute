using SilkRoute.Internal.Abstractions.Common;
using SilkRoute.Internal.HttpRequest;

namespace SilkRoute.Internal.Abstractions.HttpRequest;

internal interface IHttpRequestContentBuilder : IPrioritized
{
    bool CanBuild(HttpRequestBuilder httpRequestBuilder);

    HttpContent Build(HttpRequestBuilder httpRequestBuilder);
}