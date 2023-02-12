namespace MiniTwit.Database;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PwHash { get; set; }

    public ISet<User> Follows { get; set; }
    public ISet<User> Followers { get; set; }
}