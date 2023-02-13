using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniTwit.Repositories;

namespace MiniTwit.Pages;

public class Login : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Login(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
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
    public string? Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Username == null || Password == null) return Page();
        try
        {
            await _userRepository.Login(Username, Password);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", "You were logged in.");
            return Redirect("/");
        }
        catch (Exception e)
        {
            _httpContextAccessor.HttpContext.Session.SetString("flashes", e.Message);
            return Page();
        }
    }
}