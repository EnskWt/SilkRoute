using System.Net.Http.Headers;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory;

internal class ByteArrayRequestBodyFactory : IRequestBodyFactory
{
    public int Priority => 2;
    public bool CanCreate(object val) => val is byte[];
    public HttpContent Create(object val)
    {
        var b = (byte[])val;
        var bac = new ByteArrayContent(b);
        bac.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return bac;
    }
}