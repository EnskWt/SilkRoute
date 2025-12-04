using System.Reflection;
using Microsoft.AspNetCore.Mvc;

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
