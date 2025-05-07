using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.RequestTools;
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
