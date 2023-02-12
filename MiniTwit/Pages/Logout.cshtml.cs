using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Repositories;

namespace MiniTwit.Pages;

public class Logout : PageModel
{
    private readonly IUserRepository _userRepository;
    public Logout(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        await _userRepository.Logout();
        return Redirect("/");
    }
}