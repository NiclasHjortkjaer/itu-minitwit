using System;
using itu_minitwit.Server.Database;

namespace ituminitwit.Server.Interfaces.Repositories;

public interface IMessageRepository
{
    public Task<IEnumerable<Message>> Get(); // Gets all messages. Implement pagination later. 
    public Task<Message> Get(int id);
    public Task<IEnumerable<Message>> GetByUser(int userId);
    public Task<IEnumerable<Message>> GetByFollows(int userId);

    public bool Create(Message message);
}


