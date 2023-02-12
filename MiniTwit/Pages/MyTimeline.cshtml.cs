using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniTwit.Database;
using MiniTwit.Services;

namespace MiniTwit.Pages;

public class MyTimeline : PageModel
{
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public MyTimeline(IUserService userService, IMessageService messageService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _messageService = messageService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userService.GetCurrent();

        if (user == null) return Redirect("/public");
        
        return Page();
    }
    
    [BindProperty]
    public string? Text { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (Text != null)
        {
            await _messageService.Create(Text);
            _httpContextAccessor.HttpContext.Session.SetString("flashes", "Your message was recorded.");
        }
        
        return Redirect("/");
    }
}