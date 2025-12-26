using Microsoft.Extensions.DependencyInjection;
using SilkRoute.Abstractions.External;
using SilkRoute.Proxy;
using SilkRoute.Settings;

namespace SilkRoute.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the microservice client <typeparamref name="TClient"/> in the dependency injection container
    /// using the provided <paramref name="settings"/>.
    /// </summary>
    /// <param name="services">The service collection to add the client to.</param>
    /// <param name="settings">Configuration settings for the client registration.</param>
    /// <typeparam name="TClient">The microservice client type to register.</typeparam>
    /// <returns>The same <see cref="IServiceCollection"/> instance so calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is <c>null</c>.</exception>

    public static IServiceCollection AddMicroserviceClient<TClient>(
        this IServiceCollection services,
        MicroserviceClientSettings settings)
        where TClient : class, IMicroserviceClient
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");
        }

        var clientName = typeof(TClient).FullName ?? Guid.NewGuid().ToString();

        services.AddHttpClient(clientName, client =>
        {
            settings.HttpClientConfiguration?.Invoke(client);
        });
            
        services.AddScoped<TClient>(provider =>
        {
            var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);
            return MicroserviceProxyFactory.Create<TClient>(httpClient);
        });

        return services;
    }
}