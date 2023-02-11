namespace itu_minitwit.Client;
public class PublicHttpClient
{
    public HttpClient Client { get; }

    public PublicHttpClient(HttpClient httpClient)
    {
        Client = httpClient;
    }
}

