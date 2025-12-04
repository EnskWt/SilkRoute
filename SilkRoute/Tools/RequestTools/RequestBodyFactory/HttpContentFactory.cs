using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class HttpContentFactory : IRequestBodyFactory
    {
        public int Priority => 0;
        public bool CanCreate(object val) => val is HttpContent;
        public HttpContent Create(object val) => (HttpContent)val;
    }
}
