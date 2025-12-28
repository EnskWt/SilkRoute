using Castle.DynamicProxy;
using SilkRoute.Public.Abstractions;

namespace SilkRoute.Internal.Proxying;

internal static class MicroserviceProxyFactory
{
    private static readonly ProxyGenerator Generator = new();

    public static T Create<T>(HttpClient httpClient)
        where T : class, IMicroserviceClient
    {
        var microserviceProxy = new MicroserviceProxy<T>(httpClient);
        var interceptor = microserviceProxy.ToInterceptor();
        return Generator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
    }
}