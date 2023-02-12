using System.Net.Http.Json;
using itu_minitwit.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using static System.Net.WebRequestMethods;

namespace itu_minitwit.Client.Services;

public class MessageService : IMessageService
{
    private readonly PublicHttpClient _publicHttpClient;
    private readonly HttpClient _httpClient;

    public MessageService(PublicHttpClient publicHttpClient, HttpClient httpClient)
    {
        _publicHttpClient = publicHttpClient;
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<MessageDto>> GetTimeline()
    {
        var messages = await _publicHttpClient.Client.GetFromJsonAsync<IEnumerable<MessageDto>>("messages");
        if (messages != null)
        {
            return messages;
        }
        else
        {
            return new List<MessageDto>();
        }
    }
    
    public async Task<IEnumerable<MessageDto>> GetMyTimeline()
    {
        var messages = await _httpClient.GetFromJsonAsync<IEnumerable<MessageDto>>("messages/mytimeline");
        if (messages != null)
        {
            return messages;
        }
        else
        {
            return new List<MessageDto>();
        }
    }

    public async Task PostTweet(MessageText message)
    {
        await _httpClient.PostAsJsonAsync("messages", message);
    }
}

