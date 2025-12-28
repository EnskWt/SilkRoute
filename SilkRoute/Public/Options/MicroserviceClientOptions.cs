namespace SilkRoute.Public.Options;

public class MicroserviceClientOptions
{
    public Action<HttpClient>? HttpClientConfiguration { get; set; }
}