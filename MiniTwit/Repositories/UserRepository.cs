using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;

namespace MiniTwit.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MiniTwitContext _miniTwitContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IUserRepository> _logger;

    public UserRepository(ILogger<IUserRepository> logger, MiniTwitContext miniTwitContext, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _miniTwitContext = miniTwitContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<User> Register(string username, string email, string password, string passwordRepeat)
    {
        if (password != passwordRepeat) ThrowArgumentException(username, "The two passwords do not match.");
        if (username.Trim() != username) ThrowArgumentException(username, "Username cannot begin or end with space.");
        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9 _]+$") ||
            username.ToLower() is "public" or "login" or "register" or "logout" or "twithub" or "mytimeline") ThrowArgumentException(username, "Forbidden username.");
        if (await _miniTwitContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower())) ThrowArgumentException(username, "Username is already taken.");
        if (await _miniTwitContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower())) ThrowArgumentException(username, "Email is already taken.");

        var salt = Guid.NewGuid().ToString("N").Replace("\u0000", "");
        var user = new User() { 
            PwHash = HashPassword(password, salt), 
            Salt = salt,
            Email = email, Username = username, 
            Followers = new HashSet<User>(), 
            Follows = new HashSet<User>() 
        };

        _miniTwitContext.Users.Add(user);
        await _miniTwitContext.SaveChangesAsync();

        _logger.LogInformation("login user id: {userid} at {time}", user.Id, DateTime.UtcNow.ToLongTimeString());
        return user;
    }

    public async Task<User> Login(string username, string password)
    {
        var user = await _miniTwitContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        if (user == null) ThrowArgumentException(username, "Invalid username.");
        if (HashPassword(password, user.Salt) != user.PwHash) ThrowArgumentException(username, "Invalid password.");

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity));

        _logger.LogInformation(501, "User, id: {userid}, logged in at {time}", user.Id, DateTime.UtcNow.ToLongTimeString());
        return user;
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Logged out at {time}", DateTime.UtcNow.ToLongTimeString());
    }

    public async Task<User?> GetCurrent()
    {
        _logger.LogInformation("GetCurrent user at {time}", DateTime.UtcNow.ToLongTimeString());
        var claims = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;
        var hasId = int.TryParse(claims!.FindFirst("id")?.Value, out int id);
        if (hasId)
            return await _miniTwitContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return null;
    }

    public async Task<bool> IsFollowing(string username)
    {
        var user = await _miniTwitContext.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        var currentUser = await GetCurrent();
        if (user == null || currentUser == null) ThrowArgumentException(username, "username cannot be null");

        return user.Followers.Contains(currentUser);
    }

    public async Task ToggleFollowing(string username)
    {
        var user = await _miniTwitContext.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        var currentUser = await GetCurrent();
        if (user == null || currentUser == null) ThrowArgumentException(username, "username cannot be null");

        if (user.Followers.Contains(currentUser))
        {
            user.Followers.Remove(currentUser);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", $"You are no longer following {username}.");
            _logger.LogInformation("{username} started following {currentUser} at {time}", username, currentUser.Username, DateTime.UtcNow.ToLongTimeString());
        }
        else
        {
            user.Followers.Add(currentUser);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", $"You are now following {username}.");
            _logger.LogInformation("{username} unfollowed {currentUser} at {time}", username, currentUser.Username, DateTime.UtcNow.ToLongTimeString());
        }

        await _miniTwitContext.SaveChangesAsync();
    }

    public async Task<User?> Exists(string username)
    {
        _logger.LogInformation("Extists {username} called at {time}", username, DateTime.UtcNow.ToLongTimeString());
        return await _miniTwitContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<IEnumerable<User?>?> GetFollows(string username)
    {
        _logger.LogInformation("GetFollows {username} called at {time}", username, DateTime.UtcNow.ToLongTimeString());
        var user = await _miniTwitContext.Users
            .Include(u => u.Follows)
            .FirstOrDefaultAsync(u => string.Equals(u.Username.ToLower(), username.ToLower()));

        return user == null ? null : user.Follows;
    }

    public static string HashPassword(string password, string salt)
    {
        var hashed = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password + salt));
        return System.Text.Encoding.Default.GetString(hashed).Replace("\u0000", "");
    }
    
    private void ThrowArgumentException(string username, string errorMsg)
    {
        _logger.LogError("ArgumentException - Registering user: {username} failed at {time}. Error message: {error}", username, DateTime.UtcNow.ToLongTimeString(), errorMsg);
        throw new ArgumentException(errorMsg);
    }
}