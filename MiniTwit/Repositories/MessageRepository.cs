using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;

namespace MiniTwit.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MiniTwitContext _miniTwitContext;
    private readonly IUserRepository _userRepository;

    public MessageRepository(MiniTwitContext miniTwitContext, IUserRepository userRepository)
    {
        _miniTwitContext = miniTwitContext;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<Message>> Get()
    {
        return await _miniTwitContext.Messages
            .Include(m => m.Author)
            .OrderByDescending(m => m.PublishDate)
            // .Take(30)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetByUser(string username)
    {
        return await _miniTwitContext.Messages
            .Include(m => m.Author)
            .Where(m => m.Author.Username == username)
            .OrderByDescending(m => m.PublishDate)
            // .Take(30)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetFromFollows()
    {
        var user = await _userRepository.GetCurrent();
        var follows = (await _miniTwitContext.Users
            .Include(u => u.Follows)
            .FirstOrDefaultAsync(u => u.Id == user.Id))!.Follows;
        
        return await _miniTwitContext.Messages
            .Where(m => m.Author == user || follows.Contains(m.Author))
            .OrderByDescending(m => m.PublishDate)
            // .Take(30)
            .ToListAsync();
    }

    public async Task Create(string text)
    {
        var user = await _userRepository.GetCurrent();
        var message = new Message() { Author = user, Text = text, PublishDate = DateTime.Now, Flagged = 0 };

        _miniTwitContext.Add(message);
        await _miniTwitContext.SaveChangesAsync();
    }
}