using System.Reflection;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders.BinderContract;

internal interface IRequestParametersBinder
{
    int Priority { get; }

    bool CanBind(ParameterInfo parameterInfo, object? value);
    void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value);
}