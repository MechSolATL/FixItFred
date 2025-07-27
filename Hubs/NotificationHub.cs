using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MVP_Core.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message, string severity)
        {
            await Clients.All.SendAsync("ReceiveNotification", message, severity);
        }
    }
}
