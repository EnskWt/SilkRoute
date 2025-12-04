using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class RouteParametersBinder : AttributeParametersBinder<FromRouteAttribute>
    {
        public override int Priority { get; } = 3;

        public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            requestBuilder.RouteParams[parameterInfo.Name!] = value.ToString()!;
        }
    }
}
