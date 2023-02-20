using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;

namespace MiniTwit.Hubs;

public class TwitHub : Hub
{
    private readonly MiniTwitContext _miniTwitContext;

    public TwitHub(MiniTwitContext miniTwitContext)
    {
        _miniTwitContext = miniTwitContext;
    }
    
    public async Task JoinGroup(string groupName)
    {
        if (groupName == "mytimeline")
        {
            var claims = Context.User.Identity as ClaimsIdentity;
            var hasId = int.TryParse(claims!.FindFirst("id")?.Value, out int id);
            if (hasId)
            {
                var user = await _miniTwitContext.Users
                    .Include(u => u.Follows)
                    .FirstOrDefaultAsync(u => u.Id == id);
                foreach (var follow in user!.Follows)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, follow.Username);
                }
            }
        }
        else
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
    //Define methods for when client wants to send something to the hub via websocket
}