using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Extensions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class QueryParameterBinder : AttributeParameterBinder<FromQueryAttribute>
{
    public override int Priority => 2;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        httpRequestBuilder.QueryBuilder.AddFlattenedParameter(parameterInfo.Name!, value);
    }
}