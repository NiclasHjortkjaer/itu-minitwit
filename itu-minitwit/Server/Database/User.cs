using Microsoft.AspNetCore.Identity;

namespace itu_minitwit.Server.Database;

public class User : IdentityUser
{
    public int Id { get; set; }
    public List<User> Follows { get; set; }
    public List<User> Followers { get; set; }
}