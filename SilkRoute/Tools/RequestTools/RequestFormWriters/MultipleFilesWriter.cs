using Microsoft.AspNetCore.Http;
using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters
{
    internal class MultipleFilesWriter : IRequestFormWriter
    {
        public int Priority => 1;
        public bool CanWrite(object val) => val is IEnumerable<IFormFile>;
        public void Write(MultipartFormDataContent form, string name, object val)
        {
            foreach (var f in (IEnumerable<IFormFile>)val)
                HttpContentHelper.AddToForm(form, name, f);
        }
    }
}
