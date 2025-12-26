using SilkRoute.Tools.RequestTools.RequestContentBinder.BinderContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;
namespace SilkRoute.Tools.RequestTools.RequestContentBinder;

internal class FormParametersContentBinder : IRequestContentBinder
{
    public int Priority => 1;
    public bool CanBind(RequestBuilder requestBuilder) => requestBuilder.FormParams.Count > 0;
    public HttpContent? Bind(RequestBuilder requestBuilder)
    {
        foreach (var p in requestBuilder.NoAttributeParams.Where(p => RequestTypeHelper.ContainsNonExplicitFormData(p.Value)).ToList())
        {
            requestBuilder.FormParams.Add(p);
            requestBuilder.NoAttributeParams.Remove(p);
        }
        return HttpContentHelper.BuildFormContent(requestBuilder.FormParams);
    }
}