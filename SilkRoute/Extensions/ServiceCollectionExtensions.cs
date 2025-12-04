using Microsoft.Extensions.DependencyInjection;
using SilkRoute.Interfaces;
using SilkRoute.Proxy;
using SilkRoute.Settings;

namespace SilkRoute.Extensions
{
    public static class ServiceCollectionExtensions
    {
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

            // Register proxy for TClient
            services.AddScoped<TClient>(provider =>
            {
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);
                return MicroserviceProxyFactory<TClient>.Create(httpClient);
            });

            return services;
        }
    }
}
