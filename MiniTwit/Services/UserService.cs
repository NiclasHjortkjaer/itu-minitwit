using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;

namespace MiniTwit.Services;

public class UserService : IUserService
{
    private readonly MiniTwitContext _miniTwitContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(MiniTwitContext miniTwitContext, IHttpContextAccessor httpContextAccessor)
    {
        _miniTwitContext = miniTwitContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<User> Register(string username, string email, string password, string passwordRepeat)
    {
        if (password != passwordRepeat) throw new ArgumentException();
        if (await _miniTwitContext.Users.AnyAsync(u => u.Username == username || u.Email == email)) throw new ArgumentException();

        var user = new User() { 
            PwHash = HashPassword(password), 
            Email = email, Username = username, 
            Followers = new HashSet<User>(), 
            Follows = new HashSet<User>() 
        };

        _miniTwitContext.Users.Add(user);
        await _miniTwitContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> Login(string username, string password)
    {
        var user = await _miniTwitContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) throw new ArgumentException();
        if (HashPassword(password) != user.PwHash) throw new ArgumentException();

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity));

        return user;
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<User?> GetCurrent()
    {
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
            .FirstOrDefaultAsync(u => u.Username == username);
        var currentUser = await GetCurrent();
        if (user == null || currentUser == null) throw new ArgumentException();

        return user.Followers.Contains(currentUser);
    }

    public async Task ToggleFollowing(string username)
    {
        var user = await _miniTwitContext.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Username == username);
        var currentUser = await GetCurrent();
        if (user == null || currentUser == null) throw new ArgumentException();

        if (user.Followers.Contains(currentUser))
        {
            user.Followers.Remove(currentUser);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", $"You stopped following {username}.");
        }
        else
        {
            user.Followers.Add(currentUser);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", $"You started following {username}.");
        }

        await _miniTwitContext.SaveChangesAsync();
    }


    public static string HashPassword(string password)
    {
        HashAlgorithm sha = SHA256.Create();
        var hashed = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return System.Text.Encoding.Default.GetString(hashed);
    }
}