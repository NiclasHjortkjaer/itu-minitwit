using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class Public : PageModel
{
    private readonly IUserRepository _userRepository;
    public Public(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public void OnGet()
    {
        
    }
}