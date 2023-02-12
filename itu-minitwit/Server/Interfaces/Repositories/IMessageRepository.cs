using System;
using itu_minitwit.Server.Database;
using itu_minitwit.Shared;

namespace ituminitwit.Server.Interfaces.Repositories;

public interface IMessageRepository
{
    public Task<IEnumerable<Message>> Get(); // Gets all messages. Implement pagination later. 
    public Task<IEnumerable<Message>> GetMyTimeline();
    public Task<Message> Get(int id);
    public Task<IEnumerable<Message>> GetByUser(string username);
    public Task<IEnumerable<Message>> GetByFollows(int userId);

    public Task Create(MessageText message);
}


