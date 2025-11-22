using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using SilkRoute.Interfaces;

namespace SilkRoute.Proxy
{
    /// <summary>
    /// Factory to create proxy instances.
    /// </summary>
    internal static class MicroserviceProxyFactory<T>
        where T : class, IMicroserviceClient
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public static T Create(HttpClient httpClient)
        {
            var microserviceProxy = new MicroserviceProxy<T>(httpClient);

            var interceptor = microserviceProxy.ToInterceptor();

            var proxy = _generator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
            return proxy;
        }
    }
}
