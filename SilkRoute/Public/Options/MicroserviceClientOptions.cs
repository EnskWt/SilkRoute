namespace SilkRoute.Public.Options;

/// <summary>
///     Represents configuration options for registering a microservice client.
/// </summary>
public class MicroserviceClientOptions
{
    /// <summary>
    ///     Gets or sets a delegate used to configure the underlying <see cref="HttpClient" /> instance.
    /// </summary>
    public Action<HttpClient> HttpClientConfiguration { get; set; }
}