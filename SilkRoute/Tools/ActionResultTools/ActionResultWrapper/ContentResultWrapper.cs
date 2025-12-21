using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SilkRoute.Tools.ActionResultTools.ActionResultWrapper.WrapperContract;

namespace SilkRoute.Tools.ActionResultTools.ActionResultWrapper
{
    internal sealed class ContentResultWrapper : IActionResultWrapper
    {
        public int Priority => 50;

        public bool CanWrap(Type responseType)
            => typeof(ContentResult).IsAssignableFrom(responseType);

        public object Wrap(HttpResponseMessage response, Type responseType, object? payload)
        {
            var statusCode = (int)response.StatusCode;

            var contentType = response.Content?.Headers.ContentType?.ToString();

            return new ContentResult
            {
                StatusCode = statusCode,
                ContentType = contentType,       
                Content = payload?.ToString()  
            };
        }
    }
}
