using Microsoft.AspNetCore.Identity;

namespace itu_minitwit.Server.Database;

public class User : IdentityUser
{
    public List<User> Follows { get; set; }
    public List<User> Followers { get; set; }
}