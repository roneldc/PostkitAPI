using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Postkit.Notifications.DTOs;

namespace Postkit.Notifications.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, NotificationDto notification)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }
    }
}
