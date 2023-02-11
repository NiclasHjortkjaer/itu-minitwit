using System.Net.Http.Json;
using itu_minitwit.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using static System.Net.WebRequestMethods;

namespace itu_minitwit.Client.Services;

public class MessageService : IMessageService
{
    private readonly PublicHttpClient _httpClient;

    public MessageService(PublicHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<MessageDto>> GetTimeline()
    {
        var messages = await _httpClient.Client.GetFromJsonAsync<IEnumerable<MessageDto>>("messages");
        if (messages != null)
        {
            return messages;
        }
        else
        {
            return new List<MessageDto>();
        }
    }
}

