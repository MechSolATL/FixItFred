using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Hubs
{
    /// <summary>
    /// SignalR hub for real-time reward notifications to customers.
    /// </summary>
    public class RewardNotificationHub : Hub
    {
        public async Task SendRewardNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveRewardNotification", message);
        }
    }
}
