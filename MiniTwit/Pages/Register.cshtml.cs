using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class Register : PageModel
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Register(IUserService userService, IHttpContextAccessor httpContextAccessor)
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
    public string? Email { get; set; }
    [BindProperty]
    public string? Password { get; set; }
    [BindProperty]
    public string? PasswordRepeat { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Username != null && Email != null && Password != null && PasswordRepeat != null)
        {
            await _userService.Register(Username, Email, Password, PasswordRepeat);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", "You were successfully registered.");
            return Redirect("/login");
        }
        return Redirect("/register");
    }
}