using Castle.DynamicProxy;
using SilkRoute.Public.Abstractions;

namespace SilkRoute.Internal.Proxying;

internal static class MicroserviceClientProxyFactory
{
    private static readonly ProxyGenerator Generator = new();

    public static T Create<T>(HttpClient httpClient)
        where T : class, IMicroserviceClient
    {
        var microserviceClientInterceptor = new MicroserviceClientInterceptor(httpClient);
        var interceptor = microserviceClientInterceptor.ToInterceptor();
        return Generator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
    }
}