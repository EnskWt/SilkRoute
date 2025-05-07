using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkRoute.Tools.RequestTools.RequestContentBinder.BinderContract
{
    internal interface IRequestContentBinder
    {
        int Priority { get; }
        bool CanBind(RequestBuilder requestBuilder);
        HttpContent? Bind(RequestBuilder requestBuilder);
    }
}
