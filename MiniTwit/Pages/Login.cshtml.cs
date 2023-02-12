using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class Login : PageModel
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Login(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public void OnGet()
    {
        
    }
    
    [BindProperty]
    public string? Username { get; set; }
    [BindProperty]
    public string? Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        await _userService.Login(Username, Password);
        _httpContextAccessor.HttpContext.Session.SetString("flashes", "You were logged in.");
        return Redirect("/");
    }
}