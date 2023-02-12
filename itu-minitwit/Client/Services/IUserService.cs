using itu_minitwit.Shared;

namespace itu_minitwit.Client.Services;

public interface IUserService
{
    public Task Follow(UserName user);
    public Task UnFollow(UserName user);
    public Task<bool> IsFollowing(string userName);
}