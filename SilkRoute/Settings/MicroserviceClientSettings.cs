namespace SilkRoute.Settings;

public class MicroserviceClientSettings
{
    public Action<HttpClient>? HttpClientConfiguration { get; set; }
}