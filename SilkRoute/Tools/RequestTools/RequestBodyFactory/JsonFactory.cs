using System.Text;
using Newtonsoft.Json;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory
{
    internal class JsonFactory : IRequestBodyFactory
    {
        public int Priority => int.MaxValue;
        public bool CanCreate(object val) => true;
        public HttpContent Create(object val) => new StringContent(JsonConvert.SerializeObject(val), Encoding.UTF8, "application/json");
    }
}
