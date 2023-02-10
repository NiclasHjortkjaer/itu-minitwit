﻿using System.Net.Http.Json;
using itu_minitwit.Shared;
using static System.Net.WebRequestMethods;

namespace itu_minitwit.Client.Services;

public class MessageService : IMessageService
{
    private readonly HttpClient _httpClient;

    public MessageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<MessageDto>> GetTimeline()
    {
        var messages = await _httpClient.GetFromJsonAsync<IEnumerable<MessageDto>>("messages");
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

