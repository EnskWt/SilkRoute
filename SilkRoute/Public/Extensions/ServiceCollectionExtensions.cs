using SilkRoute.Internal.Proxying;
using SilkRoute.Public.Abstractions;
using SilkRoute.Public.Options;

namespace SilkRoute.Public.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the microservice client <typeparamref name="TClient" /> in the dependency injection container
    ///     using the provided <paramref name="options" />.
    /// </summary>
    /// <param name="services">The service collection to add the client to.</param>
    /// <param name="options">Configuration settings for the client registration.</param>
    /// <typeparam name="TClient">The microservice client type to register.</typeparam>
    /// <returns>The same <see cref="IServiceCollection" /> instance so calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options" /> is <c>null</c>.</exception>
    public static IServiceCollection AddMicroserviceClient<TClient>(
        this IServiceCollection services,
        MicroserviceClientOptions options)
        where TClient : class, IMicroserviceClient
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options), "Options cannot be null.");
        }

        var clientName = Guid.NewGuid().ToString();

        services.AddHttpClient(clientName, client =>
        {
            options.HttpClientConfiguration?.Invoke(client);
        });
            
        services.AddScoped(provider =>
        {
            var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);
            return MicroserviceClientProxyFactory.Create<TClient>(httpClient);
        });

        return services;
    }
}