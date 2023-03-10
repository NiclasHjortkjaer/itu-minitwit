using MiniTwit.Database;

namespace MiniTwit.Repositories;

public interface IUserRepository
{
    public Task<User> Register(string username, string email, string password, string passwordRepeat);
    public Task<User> Login(string username, string password);
    public Task Logout();
    public Task<User?> GetCurrent();
    public Task<bool> IsFollowing(string username);
    public Task ToggleFollowing(string username);
    public Task<User?> Exists(string username);
    public Task<IEnumerable<User?>?> GetFollows(string username);
}