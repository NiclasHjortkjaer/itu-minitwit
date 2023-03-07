using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;
using MiniTwit.Hubs;
using Prometheus;

namespace MiniTwit.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MiniTwitContext _miniTwitContext;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<TwitHub> _twitHubContext;
    private static readonly Histogram GetPublicDuration = Metrics
        .CreateHistogram("minitwit_get_timeline_duration_seconds", "Histogram of get call processing durations.");

    public MessageRepository(MiniTwitContext miniTwitContext, IUserRepository userRepository, IHubContext<TwitHub> twitHubContext)
    {
        _miniTwitContext = miniTwitContext;
        _userRepository = userRepository;
        _twitHubContext = twitHubContext;
    }
    
    public async Task<IEnumerable<Message>> Get(int? limit = null, int page = 1)
    {
        using (GetPublicDuration.NewTimer());
        
        var query = _miniTwitContext.Messages
            .Include(m => m.Author)
            .OrderByDescending(m => m.PublishDate);

        if (limit.HasValue)
        {
            query = query.Skip((page - 1) * limit.Value).Take(limit.Value) as IOrderedQueryable<Message>;
        }
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetByUser(string username, int? limit = null)
    {
        var query = _miniTwitContext.Messages
            .Include(m => m.Author)
            .Where(m => m.Author.Username.ToLower() == username.ToLower())
            .OrderByDescending(m => m.PublishDate);
        if (limit.HasValue)
        {
            query = query.Take(limit.Value) as IOrderedQueryable<Message>;
        }
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetFromFollows(int? limit = null)
    {
        var user = await _userRepository.GetCurrent();
        var follows = (await _miniTwitContext.Users
            .Include(u => u.Follows)
            .FirstOrDefaultAsync(u => u.Id == user.Id))!.Follows;

        var query = _miniTwitContext.Messages
            .Where(m => m.Author == user || follows.Contains(m.Author))
            .OrderByDescending(m => m.PublishDate);
        if (limit.HasValue)
        {
            query = query.Take(limit.Value) as IOrderedQueryable<Message>;
        }
        return await query.ToListAsync();
    }

    public async Task Create(string text)
    {
        var user = await _userRepository.GetCurrent();
        var message = new Message() { Author = user, Text = text, PublishDate = DateTime.Now, Flagged = 0 };

        _miniTwitContext.Add(message);
        await _miniTwitContext.SaveChangesAsync();
        
        await _twitHubContext.Clients.Groups("public", user.Username).SendAsync("ReceiveMessage", new
        {
            Text = message.Text,
            Username = message.Author.Username,
            PublishDate = message.PublishDate.Value.AddHours(1).ToString()
        });
    }
}