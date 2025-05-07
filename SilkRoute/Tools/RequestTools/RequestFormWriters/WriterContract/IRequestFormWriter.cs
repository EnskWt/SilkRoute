using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract
{
    internal interface IRequestFormWriter
    {
        int Priority { get; }
        bool CanWrite(object val);
        void Write(MultipartFormDataContent form, string name, object val);
    }
}
