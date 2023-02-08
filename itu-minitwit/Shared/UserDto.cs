namespace itu_minitwit.Shared;

public class UserDto 
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PwHash { get; set; }
    public List<UserDto> Follows { get; set; }
    public List<UserDto> Followers { get; set; }
}
