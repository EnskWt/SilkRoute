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
            ms.Position = 0;
            var sc = new StreamContent(ms);
            sc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = $"\"{name}\"",
                FileName = $"\"{f.FileName}\""
            };
            sc.Headers.ContentType = MediaTypeHeaderValue.Parse(f.ContentType);
            form.Add(sc, name, f.FileName);
        }
    }
}
