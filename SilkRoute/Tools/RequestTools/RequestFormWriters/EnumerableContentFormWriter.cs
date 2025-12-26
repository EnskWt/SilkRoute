using System.Collections;
using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters;

internal class EnumerableContentFormWriter : IRequestFormWriter
{
    public int Priority => 3;
    public bool CanWrite(object val) => val is IEnumerable && val is not string;
    public void Write(MultipartFormDataContent form, string name, object val)
    {
        foreach (var item in (IEnumerable)val)
            HttpContentHelper.AddToForm(form, name, item!);
    }
}