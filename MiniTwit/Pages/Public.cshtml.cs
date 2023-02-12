using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class Public : PageModel
{
    private readonly IUserService _userService;
    public Public(IUserService userService)
    {
        _userService = userService;
    }
    
    public void OnGet()
    {
        
    }
}