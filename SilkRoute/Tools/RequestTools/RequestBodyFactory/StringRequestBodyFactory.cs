using System.Text;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class StringRequestBodyFactory : IRequestBodyFactory
    {
        public int Priority => 3;
        public bool CanCreate(object val) => val is string;
        public HttpContent Create(object val) => new StringContent((string)val, Encoding.UTF8, "text/plain");
    }
}
