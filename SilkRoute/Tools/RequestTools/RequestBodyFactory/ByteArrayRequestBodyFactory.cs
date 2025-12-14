using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class ByteArrayRequestBodyFactory : IRequestBodyFactory
    {
        public int Priority => 2;
        public bool CanCreate(object val) => val is byte[];
        public HttpContent Create(object val) => new ByteArrayContent((byte[])val);
    }
}
