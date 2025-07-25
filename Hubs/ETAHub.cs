// FixItFred Patch Log — Sprint 29A: ETA SignalR Hub Scaffolding
// [2025-07-25T00:00:00Z] — Initial ETAHub scaffold with BroadcastETA method for live ETA push.
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MVP_Core.Hubs
{
    public class ETAHub : Hub
    {
        public async Task BroadcastETA(int technicianId, string eta)
        {
            // Broadcast ETA update to all connected clients
            await Clients.All.SendAsync("ReceiveETA", technicianId, eta);
        }
    }
}
