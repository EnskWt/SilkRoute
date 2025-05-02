using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Extensions.HttpRequest;
using SilkRoute.Internal.Extensions.ModelBinding;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal sealed class QueryParameterBinder : AttributeParameterBinder<FromQueryAttribute>
{
    public override int Priority => 20;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var queryParameterName = parameterInfo.GetModelBindingNameOrDefault<FromQueryAttribute>();

        httpRequestBuilder.QueryBuilder.AddFlattenedParameter(queryParameterName, value);
    }
}