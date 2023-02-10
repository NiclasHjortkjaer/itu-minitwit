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

    public Task<IEnumerable<Message>> GetByUser(int userId)
    {
        throw new NotImplementedException();
    }
}