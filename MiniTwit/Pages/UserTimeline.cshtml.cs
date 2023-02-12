using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class UserTimeline : PageModel
{
    private readonly IUserRepository _userRepository;
    public UserTimeline(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public string? Username { get; set; }
    public void OnGet(string? username)
    {
        Username = username;
    }

    public async Task<IActionResult> OnPostAsync(string? username)
    {
        await _userRepository.ToggleFollowing(username);

        return Redirect($"/{username}");
    }
}