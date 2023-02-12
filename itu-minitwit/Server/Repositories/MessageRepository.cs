using System.Security.Claims;
using Azure;
using itu_minitwit.Server.Database;
using itu_minitwit.Shared;
using ituminitwit.Server.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace itu_minitwit.Server.Repositories;

public class MessageRepository : IMessageRepository
{
    private MinitwitContext _minitwitContext;
    private IHttpContextAccessor _contextAccessor;


    public MessageRepository(MinitwitContext minitwitContext, IHttpContextAccessor contextAccessor)
    {
        _minitwitContext = minitwitContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<IEnumerable<Message>> Get()
    {
        return await _minitwitContext.Messages
            .Include(m => m.Author)
            .OrderByDescending(m => m.PublishDate)
            // .Take(30)
            .ToListAsync();
    }
    public async Task<IEnumerable<Message>> GetMyTimeline()
    {
        var claimsIdentity = (ClaimsIdentity)_contextAccessor.HttpContext.User.Identity;
        var userIdClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        return await _minitwitContext.Messages
        .Select(m => m)
        .Where(m => m.AuthorId == userIdClaim!.Value)
        .Include(m => m.Author)
        .OrderByDescending(m => m.PublishDate)
        // .Take(30)
        .ToListAsync();
    }

    public async Task Create(MessageText messageText)
    {
        
        var claimsIdentity = (ClaimsIdentity)_contextAccessor.HttpContext.User.Identity;

        var userIdClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
  
        var message = new Message()
        {
            AuthorId = userIdClaim!.Value,
            Text = messageText.Text,
            PublishDate = DateTime.Now
        };

        _minitwitContext.Messages.Add(message);
        await _minitwitContext.SaveChangesAsync();
    }


    public Task<Message> Get(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Message>> GetByFollows(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Message>> GetByUser(string username)
    {
        return await _minitwitContext.Messages
            .Include(m => m.Author)
            .Where(m => m.Author.UserName == username)
            .OrderByDescending(m => m.PublishDate)
            // .Take(30)
            .ToListAsync();
    }
}