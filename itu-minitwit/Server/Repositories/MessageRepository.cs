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
            .OrderByDescending(m => m.PublishDate)
            .Include(m => m.Author)
            // .Take(30)
            .ToListAsync();
    }

    public bool Get(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<bool> GetByUser(int userId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<bool> GetByFollows(int userId)
    {
        throw new NotImplementedException();
    }

    public bool Create(bool message)
    {
        throw new NotImplementedException();
    }
}