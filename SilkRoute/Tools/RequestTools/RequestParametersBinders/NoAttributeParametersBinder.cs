using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools;
using SilkRoute.Tools.RequestTools.RequestParametersBinders.BinderContract;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class NoAttributeParametersBinder : IRequestParametersBinder
    {
        public int Priority { get; } = int.MaxValue;
        public bool CanBind(ParameterInfo p, object? value) => value != null;

        public void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            requestBuilder.NoAttributeParams.Add((parameterInfo.Name!, value));
        }
    }
}
