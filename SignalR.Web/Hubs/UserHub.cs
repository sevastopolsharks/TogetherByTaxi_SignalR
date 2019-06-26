using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Web.Hubs
{
    [Authorize]
    public class UserHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
            await Clients.All.SendAsync("connected", $"SignalR: {Context.User.Identity.Name}, вошел в систему");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);

            await Clients.All.SendAsync("disconnected", $"SignalR: {Context.User.Identity.Name}, вышел из системы");
        }
    }
}
