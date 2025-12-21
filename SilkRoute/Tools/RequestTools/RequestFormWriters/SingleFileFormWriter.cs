using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters
{
    internal class SingleFileFormWriter : IRequestFormWriter
    {
        public int Priority => 0;
        public bool CanWrite(object val) => val is IFormFile;

        public void Write(MultipartFormDataContent form, string name, object val)
        {
            var f = (IFormFile)val;

            using var ms = new MemoryStream();
            f.CopyTo(ms);
            var bytes = ms.ToArray();

            var content = new ByteArrayContent(bytes);

            var contentType = string.IsNullOrWhiteSpace(f.ContentType)
                ? "application/octet-stream"
                : f.ContentType;

            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            form.Add(content, name, f.FileName);
        }
    }
}
