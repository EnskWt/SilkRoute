using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class HttpContentFactory : IRequestBodyFactory
    {
        public int Priority => 0;
        public bool CanHandle(object val) => val is HttpContent;
        public HttpContent Create(object val) => (HttpContent)val;
    }
}
