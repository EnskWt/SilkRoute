using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.RequestTools.RequestParametersBinders;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class ServiceParametersBinder : AttributeParametersBinder<FromServicesAttribute>
    {
        public override int Priority { get; } = 0;

        public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            return;
        }
    }
}
