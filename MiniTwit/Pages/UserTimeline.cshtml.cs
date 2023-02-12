using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class UserTimeline : PageModel
{
    private readonly IUserService _userService;
    public UserTimeline(IUserService userService)
    {
        _userService = userService;
    }
    public string? Username { get; set; }
    public void OnGet(string? username)
    {
        Username = username;
    }

    public async Task<IActionResult> OnPostAsync(string? username)
    {
        await _userService.ToggleFollowing(username);

        return Redirect($"/{username}");
    }
}