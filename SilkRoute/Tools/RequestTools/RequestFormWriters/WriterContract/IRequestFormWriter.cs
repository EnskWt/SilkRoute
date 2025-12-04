namespace SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract
{
    internal interface IRequestFormWriter
    {
        int Priority { get; }
        bool CanWrite(object val);
        void Write(MultipartFormDataContent form, string name, object val);
    }
}
