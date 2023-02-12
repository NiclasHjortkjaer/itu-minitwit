using System.Security.Claims;
using itu_minitwit.Server.Database;
using ituminitwit.Server.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace itu_minitwit.Server.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MinitwitContext _minitwitContext;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly UserManager<User> _userManager;
    public UserRepository(MinitwitContext minitwitContext, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
    {
        _minitwitContext = minitwitContext;
        _contextAccessor = contextAccessor;
        _userManager = userManager;
    }
    
    public Task<IEnumerable<User>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<User> Get(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByName(string name)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Create(User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateCredentials(string name, string hashed_password)
    {
        throw new NotImplementedException();
    }

    public async Task Follow(string username)
    {
        var currentUser = await GetCurrentUser();
        
        var user = await _minitwitContext.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user != null && currentUser != null)
        {
            if (!user.Followers.Contains(user))
            {
                user.Followers.Add(user);
                await _minitwitContext.SaveChangesAsync();   
            }
        }
    }

    public async Task Unfollow(string username)
    {
        var currentUser = await GetCurrentUser();
        
        var user = await _minitwitContext.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user != null && currentUser != null)
        {
            if (user.Followers.Contains(user))
            {
                user.Followers.Remove(user);
                await _minitwitContext.SaveChangesAsync();
            }
            
        }
    }

    public async Task<bool> IsFollowing(string username)
    {
        var currentUser = await GetCurrentUser();
        
        var user = await _minitwitContext.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user != null && currentUser != null)
        {
            return user.Followers.Contains(currentUser);
        }

        return false;
    }

    public async Task<User?> GetCurrentUser()
    {
        var claimsIdentity = (ClaimsIdentity)_contextAccessor.HttpContext.User.Identity;
        var userId = claimsIdentity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        return await _userManager.Users.Select(u => u).Where(u => u.Id == userId).FirstOrDefaultAsync();
    }
}