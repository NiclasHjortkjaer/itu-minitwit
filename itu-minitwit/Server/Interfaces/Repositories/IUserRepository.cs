using System;
namespace ituminitwit.Server.Interfaces.Repositories;

public interface IUserRepository
{
    public bool Get(int id);
    public bool GetByName(string name); //might be redundant later, depends on implementation
    public bool Create(bool user);    // bool = user object
    public bool ValidateCredentials(string name, string hashed_password); 
    public bool Follow(int whoId, int whomId);
    public bool Unfollow(int whoId, int whomId);
}
