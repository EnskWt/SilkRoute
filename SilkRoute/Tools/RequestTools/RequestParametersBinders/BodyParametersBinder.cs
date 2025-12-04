using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class BodyParametersBinder : AttributeParametersBinder<FromBodyAttribute>
    {
        public override int Priority { get; } = 4;

        public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            requestBuilder.ExplicitBody = (parameterInfo.Name!, value);
        }
    }
}
