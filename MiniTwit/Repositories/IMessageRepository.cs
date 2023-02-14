using MiniTwit.Database;

namespace MiniTwit.Repositories;

public interface IMessageRepository
{
    public Task<IEnumerable<Message>> Get();
    public Task<IEnumerable<Message>> GetByUser(string username);
    public Task<IEnumerable<Message>> GetFromFollows();
    public Task Create(string text);
}