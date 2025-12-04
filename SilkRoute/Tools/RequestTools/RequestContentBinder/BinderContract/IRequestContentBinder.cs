namespace SilkRoute.Tools.RequestTools.RequestContentBinder.BinderContract
{
    internal interface IRequestContentBinder
    {
        int Priority { get; }
        bool CanBind(RequestBuilder requestBuilder);
        HttpContent? Bind(RequestBuilder requestBuilder);
    }
}
