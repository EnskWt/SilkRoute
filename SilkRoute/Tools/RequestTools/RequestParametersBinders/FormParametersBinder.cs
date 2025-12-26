using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders;

internal class FormParametersBinder : AttributeParametersBinder<FromFormAttribute>
{
    public override int Priority { get; } = 5;

    public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
    {
        requestBuilder.FormParams.Add((parameterInfo.Name!, value));
    }
}