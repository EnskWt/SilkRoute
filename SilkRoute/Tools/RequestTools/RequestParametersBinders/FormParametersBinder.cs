using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SilkRoute.Tools.RequestTools;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class FormParametersBinder : AttributeParametersBinder<FromFormAttribute>
    {
        public override int Priority { get; } = 5;

        public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            requestBuilder.FormParams.Add((parameterInfo.Name!, value));
        }
    }
}
