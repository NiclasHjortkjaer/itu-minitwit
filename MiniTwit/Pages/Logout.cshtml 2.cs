using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class Logout : PageModel
{
    private readonly IUserService _userService;
    public Logout(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        await _userService.Logout();
        return Redirect("/");
    }
}