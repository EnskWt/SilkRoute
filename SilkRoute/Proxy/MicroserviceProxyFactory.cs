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
    public static class MicroserviceProxyFactory<T> where T : IMicroserviceClient
    {
        public static T Create(HttpClient httpClient)
        {
            object proxy = DispatchProxy.Create<T, MicroserviceProxy<T>>();
            ((MicroserviceProxy<T>)proxy).SetHttpClient(httpClient);
            return (T)proxy;
        }
    }
}
