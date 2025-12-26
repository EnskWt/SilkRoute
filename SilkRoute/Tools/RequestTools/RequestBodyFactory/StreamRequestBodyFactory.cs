using System.Net.Http.Headers;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory;

internal class StreamRequestBodyFactory : IRequestBodyFactory
{
    public int Priority => 1;
    public bool CanCreate(object val) => val is Stream;
    public HttpContent Create(object val)
    {
        var s = (Stream)val;
        var sc = new StreamContent(s);
        sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return sc;
    }
}