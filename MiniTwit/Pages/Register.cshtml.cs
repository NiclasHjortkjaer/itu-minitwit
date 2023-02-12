using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Repositories;

namespace MiniTwit.Pages;

public class Register : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Register(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
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
            await _userRepository.Register(Username, Email, Password, PasswordRepeat);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", "You were successfully registered.");
            return Redirect("/login");
        }
        return Redirect("/register");
    }
}