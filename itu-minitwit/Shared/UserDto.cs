namespace itu_minitwit.Shared;

public class UserDto 
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<UserDto> Follows { get; set; }
    public List<UserDto> Followers { get; set; }
}
