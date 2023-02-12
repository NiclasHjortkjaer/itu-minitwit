using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Database;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class MyTimeline : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public MyTimeline(IUserRepository userRepository, IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userRepository.GetCurrent();

        if (user == null) return Redirect("/public");
        
        return Page();
    }
    
    [BindProperty]
    public string? Text { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (Text != null)
        {
            await _messageRepository.Create(Text);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", "Your message was recorded.");
        }
        
        return Redirect("/");
    }
}