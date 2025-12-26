using SilkRoute.Tools.RequestTools.RequestContentBinder.BinderContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestContentBinder;

internal class ExplicitBodyContentBinder : IRequestContentBinder
{
    public int Priority => 0;
    public bool CanBind(RequestBuilder requestBuilder) => requestBuilder.ExplicitBody.HasValue;
    public HttpContent? Bind(RequestBuilder requestBuilder) => HttpContentHelper.BuildBodyContent(requestBuilder.ExplicitBody!.Value.Value);
}