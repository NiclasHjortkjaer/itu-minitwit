namespace itu_minitwit.Server.Database;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PwHash { get; set; }
    
    public List<User> Follows { get; set; }
    public List<User> Followers { get; set; }
}