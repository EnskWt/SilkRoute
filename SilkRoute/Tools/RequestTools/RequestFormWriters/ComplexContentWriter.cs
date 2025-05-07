using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters
{
    internal class ComplexContentWriter  : IRequestFormWriter
    {
        public int Priority => 4;

        public bool CanWrite(object val)
            => !RequestTypeHelper.IsPrimitive(val.GetType()) && val is not string && val is not IEnumerable;

        public void Write(MultipartFormDataContent form, string name, object val)
        {
            foreach (var prop in val.GetType()
                                   .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetIndexParameters().Length > 0) continue;
                var pv = prop.GetValue(val);
                if (pv != null)
                    HttpContentHelper.AddToForm(form, prop.Name, pv);
            }
        }
    }
}
