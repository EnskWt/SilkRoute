using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.RequestTools;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class HeaderParametersBinder : AttributeParametersBinder<FromHeaderAttribute>
    {
        public override int Priority { get; } = 1;

        public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            requestBuilder.Headers[parameterInfo.Name!] = value.ToString()!;
        }
    }
}
