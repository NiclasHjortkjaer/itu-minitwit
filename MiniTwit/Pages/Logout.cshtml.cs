using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Repositories;

namespace MiniTwit.Pages;

public class Logout : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Logout(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        await _userRepository.Logout();
        _httpContextAccessor.HttpContext.Session.SetString("flashes", "You were logged out.");
        return Redirect("/");
    }
}