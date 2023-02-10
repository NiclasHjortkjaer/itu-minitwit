using System;
using itu_minitwit.Server.Database;

namespace ituminitwit.Server.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> Get();
    public Task<User> Get(int id);
    public Task<User> GetByName(string name); //might be redundant later, depends on implementation
    public Task<bool> Create(User user);    
    public Task<bool> ValidateCredentials(string name, string hashed_password); 
    public Task<bool> Follow(int whoId, int whomId);
    public Task<bool> Unfollow(int whoId, int whomId);
}
