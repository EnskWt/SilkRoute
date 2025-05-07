using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters
{
    internal class PrimitiveContentWriter : IRequestFormWriter
    {
        public int Priority => 2;
        public bool CanWrite(object val) => RequestTypeHelper.IsPrimitive(val.GetType());
        public void Write(MultipartFormDataContent form, string name, object val) => form.Add(new StringContent(val.ToString()!), name);
    }
}
