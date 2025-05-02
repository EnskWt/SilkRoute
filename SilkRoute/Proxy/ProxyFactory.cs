using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SilkRoute.Interfaces;

namespace SilkRoute.Proxy
{
    /// <summary>
    /// Factory to create proxy instances.
    /// </summary>
    public static class ProxyFactory<T> where T : IMicroserviceClient
    {
        public static T Create(HttpClient httpClient)
        {
            object proxy = DispatchProxy.Create<T, Proxy<T>>();
            ((Proxy<T>)proxy).SetHttpClient(httpClient);
            return (T)proxy;
        }
    }
}
