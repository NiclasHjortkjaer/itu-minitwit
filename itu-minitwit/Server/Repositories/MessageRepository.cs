using itu_minitwit.Server.Database;
using ituminitwit.Server.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace itu_minitwit.Server.Repositories;

public class MessageRepository : IMessageRepository
{
    private MinitwitContext _minitwitContext;

    public MessageRepository(MinitwitContext minitwitContext)
    {
        _minitwitContext = minitwitContext;
    }

    public async Task<IEnumerable<Message>> Get()
    {
        return await _minitwitContext.Messages
            .Include(m => m.Author)
            .OrderByDescending(m => m.PublishDate)
            // .Take(30)
            .ToListAsync();
    }

    public bool Create(Message message)
    {
        throw new NotImplementedException();
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