using System.Net.Http.Json;
using itu_minitwit.Shared;

namespace itu_minitwit.Client.Services;

public class UserService : IUserService
{
    private readonly PublicHttpClient _publicHttpClient;
    private readonly HttpClient _httpClient;

    public UserService(PublicHttpClient publicHttpClient, HttpClient httpClient)
    {
        _publicHttpClient = publicHttpClient;
        _httpClient = httpClient;
    }
    
    public async Task Follow(UserName userName)
    {
        await _httpClient.PutAsJsonAsync("users/follow", userName);
    }
    public async Task UnFollow(UserName userName)
    {
        await _httpClient.PutAsJsonAsync("users/unfollow", userName);
    }
    
    public async Task<bool> IsFollowing(string userName)
    {
        return await _httpClient.GetFromJsonAsync<bool>("users/" + userName);
    }
}