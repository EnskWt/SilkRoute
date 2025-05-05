using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace SilkRoute.InputFormatters
{
    public class AnyStreamInputFormatter : InputFormatter
    {
        private static readonly string[] _supportedTypes = new[]
        {
            "application/octet-stream",
            "application/pdf",
            "application/zip",
            "image/*",
            "video/*",
            "audio/*",
            "multipart/form-data"
        };

        public AnyStreamInputFormatter()
        {
            foreach (var mediaType in _supportedTypes)
                SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
        }

        public override bool CanRead(InputFormatterContext context)
        {
            return context.ModelType == typeof(Stream);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            return InputFormatterResult.SuccessAsync(context.HttpContext.Request.Body);
        }
    }
}
