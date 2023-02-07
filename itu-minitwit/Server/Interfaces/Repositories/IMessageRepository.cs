using System;
using itu_minitwit.Server.Database;

namespace ituminitwit.Server.Interfaces.Repositories;

public interface IMessageRepository
{
    public Task<IEnumerable<Message>> Get(); // Gets all messages. Implement pagination later. 
    public bool Get(int id);
    public IEnumerable<bool> GetByUser(int userId);
    public IEnumerable<bool> GetByFollows(int userId);

    public bool Create(bool message); // bool = message object
}


