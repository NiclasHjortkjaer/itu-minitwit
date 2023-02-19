using Microsoft.AspNetCore.SignalR;

namespace MiniTwit.Hubs;

public class TwitHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
    //Define methods for when client wants to send something to the hub via websocket
}