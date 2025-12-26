using System.Reflection;
using SilkRoute.Tools.RequestTools.RequestParametersBinders.BinderContract;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders;

internal class NoAttributeParametersBinder : IRequestParametersBinder
{
    public int Priority { get; } = int.MaxValue;
    public bool CanBind(ParameterInfo parameterInfo, object? value) => value != null;

    public void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
    {
        requestBuilder.NoAttributeParams.Add((parameterInfo.Name!, value));
    }
}