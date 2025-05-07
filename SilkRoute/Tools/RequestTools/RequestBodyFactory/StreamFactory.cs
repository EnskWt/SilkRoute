using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class StreamFactory : IRequestBodyFactory
    {
        public int Priority => 1;
        public bool CanHandle(object val) => val is Stream;
        public HttpContent Create(object val)
        {
            var s = (Stream)val;
            var sc = new StreamContent(s);
            sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return sc;
        }
    }
}
