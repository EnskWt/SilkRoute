using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class StringFactory : IRequestBodyFactory
    {
        public int Priority => 3;
        public bool CanCreate(object val) => val is string;
        public HttpContent Create(object val) => new StringContent((string)val, Encoding.UTF8, "text/plain");
    }
}
