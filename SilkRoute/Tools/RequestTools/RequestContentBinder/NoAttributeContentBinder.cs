using SilkRoute.Tools.RequestTools.RequestContentBinder.BinderContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestContentBinder;

internal class NoAttributeContentBinder : IRequestContentBinder
{
    public int Priority => int.MaxValue;

    private delegate bool CanHandleDelegate(RequestBuilder requestBuilder);
    private delegate HttpContent? BuildContentDelegate(RequestBuilder requestBuilder);

    private readonly List<(CanHandleDelegate CanHandle, BuildContentDelegate BuildContent)> _handlers;

    public NoAttributeContentBinder()
    {
        _handlers = new List<(CanHandleDelegate, BuildContentDelegate)>
        {
            (CanHandleForm, BuildFormContent),
            (CanHandleBody, BuildBodyContent)
        };
    }

    public bool CanBind(RequestBuilder requestBuilder) => _handlers.Any(h => h.CanHandle(requestBuilder));

    public HttpContent? Bind(RequestBuilder requestBuilder)
    {
        var handler = _handlers.First(h => h.CanHandle(requestBuilder));
        return handler.BuildContent(requestBuilder);
    }

    private bool CanHandleForm(RequestBuilder requestBuilder) => requestBuilder.NoAttributeParams.Any(p => RequestTypeHelper.ContainsNonExplicitFormData(p.Value));

    private HttpContent? BuildFormContent(RequestBuilder requestBuilder)
    {
        var nonExplicitFormParams = requestBuilder.NoAttributeParams
            .Where(p => RequestTypeHelper.ContainsNonExplicitFormData(p.Value))
            .ToList();

        foreach (var p in nonExplicitFormParams)
        {
            requestBuilder.NoAttributeParams.Remove(p);
        }

        return HttpContentHelper.BuildFormContent(nonExplicitFormParams);
    }

    private bool CanHandleBody(RequestBuilder requestBuilder) => requestBuilder.NoAttributeParams.Any(p => !RequestTypeHelper.IsPrimitive(p.Value.GetType()) && !RequestTypeHelper.ContainsNonExplicitFormData(p.Value));

    private HttpContent? BuildBodyContent(RequestBuilder requestBuilder)
    {
        var firstBodyParam = requestBuilder.NoAttributeParams.First(p => !RequestTypeHelper.IsPrimitive(p.Value.GetType()) && !RequestTypeHelper.ContainsNonExplicitFormData(p.Value));
        requestBuilder.NoAttributeParams.Remove(firstBodyParam);
        return HttpContentHelper.BuildBodyContent(firstBodyParam.Value);
    }
}