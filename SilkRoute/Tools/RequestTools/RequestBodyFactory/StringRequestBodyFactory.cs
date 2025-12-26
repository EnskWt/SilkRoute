using System.Text;
using Newtonsoft.Json;
using SilkRoute.Tools.RequestTools.RequestBodyFactory.FactoryContract;

namespace SilkRoute.Tools.RequestTools.RequestBodyFactory;

internal class StringRequestBodyFactory : IRequestBodyFactory
{
    public int Priority => 3;
    public bool CanCreate(object val) => val is string;
    public HttpContent Create(object val) => new StringContent(JsonConvert.SerializeObject((string)val), Encoding.UTF8, "application/json");
}