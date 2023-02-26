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
        //This is duplicated from SimulatorController errors should be moved into the repository method
        string error = null;
        if (Username == null)
        {
            error = "You have to enter a username";
        } else if (Email == null || !Email.Contains("@"))
        {
            error = "You have to enter a valid email address";
        } else if (Password == null)
        {
            error = "You have to enter a password";
        } else if (await _userRepository.Exists(Username) != null)
        {
            error = "The username is already taken";
        }

        if (error != null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("flashes", error);
            return Page();
        }
        if (Username != null && Email != null && Password != null && PasswordRepeat != null)
        {
            try
            {
                await _userRepository.Register(Username, Email, Password, PasswordRepeat);
                _httpContextAccessor.HttpContext.Session.SetString("flashes", "You were successfully registered and can login now.");
                return Redirect("/login");

            }
            catch (Exception e)
            {
                _httpContextAccessor.HttpContext.Session.SetString("flashes", e.Message);
                return Page();
            }
        }
        return Page();
    }
}