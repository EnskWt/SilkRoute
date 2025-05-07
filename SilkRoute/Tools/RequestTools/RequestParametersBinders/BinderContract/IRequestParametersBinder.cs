using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders.BinderContract
{
    internal interface IRequestParametersBinder
    {
        int Priority { get; }

        bool CanBind(ParameterInfo parameterInfo, object? value);
        void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value);
    }
}
