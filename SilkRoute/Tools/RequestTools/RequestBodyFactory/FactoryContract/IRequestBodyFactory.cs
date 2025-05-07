using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract
{
    internal interface IRequestBodyFactory
    {
        int Priority { get; }
        bool CanHandle(object val);
        HttpContent Create(object val);
    }
}
