using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Internal.Extensions.HttpRequest;

namespace SilkRoute.Internal.HttpRequest.HttpRequestParameterBinders;

internal class QueryParameterBinder : AttributeParameterBinder<FromQueryAttribute>
{
    public override int Priority => 20;

    public override void Bind(HttpRequestBuilder httpRequestBuilder, ParameterInfo parameterInfo, object value)
    {
        var fromQuery = parameterInfo.GetCustomAttribute<FromQueryAttribute>(inherit: true);

        var queryParameterName =
            !string.IsNullOrWhiteSpace(fromQuery?.Name)
                ? fromQuery.Name!
                : parameterInfo.Name!;
        
        httpRequestBuilder.QueryBuilder.AddFlattenedParameter(queryParameterName, value);
    }
}