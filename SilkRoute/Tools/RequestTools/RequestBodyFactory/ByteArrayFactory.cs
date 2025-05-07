using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class ByteArrayFactory : IRequestBodyFactory
    {
        public int Priority => 2;
        public bool CanHandle(object val) => val is byte[];
        public HttpContent Create(object val) => new ByteArrayContent((byte[])val);
    }
}
