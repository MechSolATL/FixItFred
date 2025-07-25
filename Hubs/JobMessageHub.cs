using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MVP_Core.Hubs
{
    // FixItFred — Sprint 46.1 Build Stabilization
    public class JobMessageHub : Hub
    {
        // Stub for SignalR hub used by job messaging features
        public async Task SendMessage(int requestId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", requestId, message);
        }
    }
}
