using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestParametersBinders
{
    internal class QueryParametersBinder : AttributeParametersBinder<FromQueryAttribute>
    {
        public override int Priority { get; } = 2;

        public override void Bind(RequestBuilder requestBuilder, ParameterInfo parameterInfo, object value)
        {
            QueryParameterHelper.AddQueryParams(requestBuilder.QueryBuilder, parameterInfo.Name!, value);
        }
    }
}
