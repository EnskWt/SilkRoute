using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper;

internal sealed class ObjectResultWrapper : IActionResultWrapper
{
    public int Priority => 10;

    public bool CanWrap(Type responseType)
        => typeof(ObjectResult).IsAssignableFrom(responseType);

    public object Wrap(HttpResponseMessage response, Type responseType, object? payload)
    {
        var statusCode = (int)response.StatusCode;
        var contentType = response.Content?.Headers.ContentType?.ToString();

        var obj = (ObjectResult)Activator.CreateInstance(responseType, payload)!;

        obj.StatusCode = statusCode;

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            obj.ContentTypes.Clear();
            obj.ContentTypes.Add(contentType);
        }

        if (obj.Value == null && payload != null)
            obj.Value = payload;

        return obj;
    }
}