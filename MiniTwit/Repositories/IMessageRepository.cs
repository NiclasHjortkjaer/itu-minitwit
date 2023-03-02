using MiniTwit.Database;

namespace MiniTwit.Repositories;

public interface IMessageRepository
{
    public Task<IEnumerable<Message>> Get(int? limit = null, int page = 1);
    public Task<IEnumerable<Message>> GetByUser(string username, int? limit = null);
    public Task<IEnumerable<Message>> GetFromFollows(int? limit = null);
    public Task Create(string text);
}