using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;
using SilkRoute.Tools.RequestTools.RequestHelpers;

namespace SilkRoute.Tools.RequestTools.RequestFormWriters;

internal class PrimitiveContentFormWriter : IRequestFormWriter
{
    public int Priority => 2;
    public bool CanWrite(object val) => RequestTypeHelper.IsPrimitive(val.GetType());
    public void Write(MultipartFormDataContent form, string name, object val) => form.Add(new StringContent(val.ToString()!), name);
}