using SilkRoute.Tools.RequestTools.RequestBodyFactory;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;
using SilkRoute.Tools.RequestTools.RequestFormWriters;
using SilkRoute.Tools.RequestTools.RequestFormWriters.WriterContract;

namespace SilkRoute.Tools.RequestTools.RequestHelpers
{
    internal class HttpContentHelper
    {
        private static readonly List<IRequestBodyFactory> _bodyFactories = new List<IRequestBodyFactory>
        {
            new StreamRequestBodyFactory(),
            new ByteArrayRequestBodyFactory(),
            new StringRequestBodyFactory(),
            new JsonRequestBodyFactory()
        }.OrderBy(x => x.Priority).ToList();

        private static readonly List<IRequestFormWriter> _formWriters = new List<IRequestFormWriter>
        {
            new SingleFileFormWriter(),
            new MultipleFilesFormWriter(),
            new PrimitiveContentFormWriter(),
            new EnumerableContentFormWriter(),
            new ComplexContentFormWriter()
        }.OrderBy(x => x.Priority).ToList();

        internal static HttpContent BuildBodyContent(object val)
        {
            if (RequestTypeHelper.ContainsNonExplicitFormData(val) || RequestTypeHelper.ContainsNestedStream(val))
                throw new InvalidOperationException(
                    $"Parameter of type '{val.GetType().Name}' contains a Stream or Form-Data. " +
                    "You must pass streams either as top-level [FromBody] (Stream) or [FromForm] (IFormFile), " +
                    "but not embed them in a complex DTO.");

            var factory = _bodyFactories.First(x => x.CanCreate(val));
            return factory.Create(val);
        }

        internal static HttpContent BuildFormContent(IEnumerable<(string Name, object Value)> items)
        {
            var multipart = new MultipartFormDataContent();
            foreach (var (name, value) in items)
                AddToForm(multipart, name, value);

            return multipart;
        }

        internal static void AddToForm(MultipartFormDataContent form, string name, object? val)
        {
            if (val == null) return;

            var writer = _formWriters.First(x => x.CanWrite(val));
            writer.Write(form, name, val);
        }
    }
}
