using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
            new HttpContentFactory(),
            new StreamFactory(),
            new ByteArrayFactory(),
            new StringFactory(),
            new JsonFactory()
        }.OrderBy(x => x.Priority).ToList();

        private static readonly List<IRequestFormWriter> _formWriters = new List<IRequestFormWriter>
        {
            new SingleFileWriter(),
            new MultipleFilesWriter(),
            new PrimitiveContentWriter(),
            new EnumerableContentWriter(),
            new ComplexContentWriter()
        }.OrderBy(x => x.Priority).ToList();

        internal static HttpContent BuildBodyContent(object val)
        {
            if (RequestTypeHelper.ContainsNonExplicitFormData(val) || RequestTypeHelper.ContainsNestedStream(val))
                throw new InvalidOperationException(
                    $"Parameter of type '{val.GetType().Name}' contains a Stream or Form-Data. " +
                    "You must pass streams either as top-level [FromBody] (Stream) or [FromForm] (IFormFile), " +
                    "but not embed them in a complex DTO.");

            var factory = _bodyFactories.First(x => x.CanHandle(val));
            return factory.Create(val);
        }

        internal static HttpContent BuildFormContent(IEnumerable<(string Name, object Value)> items)
        {
            var multipart = new MultipartFormDataContent();
            foreach (var (name, value) in items)
                AddToForm(multipart, name, value);

            var bytes = multipart.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            var bc = new ByteArrayContent(bytes);
            bc.Headers.TryAddWithoutValidation("Content-Type", multipart.Headers.ContentType?.ToString());
            return bc;
        }

        internal static void AddToForm(MultipartFormDataContent form, string name, object? val)
        {
            if (val == null) return;

            var writer = _formWriters.First(x => x.CanWrite(val));
            writer.Write(form, name, val);
        }
    }
}
