// RequestHub.cs
// SignalR hub to broadcast new or reassigned service requests to connected dispatcher clients

using Microsoft.AspNetCore.SignalR;
using MVP_Core.Services.Admin;
using System.Threading.Tasks;

namespace MVP_Core.Hubs
{
    public class RequestHub : Hub
    {
        // Future: support group joins or role-targeted dispatching
        // e.g., await Groups.AddToGroupAsync(Context.ConnectionId, "Dispatchers");

        // Push live ETA updates to dispatcher view
        public async Task PushLiveETAUpdate(int jobId, string zone, string message)
        {
            await Clients.All.SendAsync("ReceiveETA", zone, message);
        }

        // Sprint 48.1 – SmartQueue ETA Broadcast
        // Broadcast Smart ETA suggestions to Dispatcher UI
        public async Task OnSmartETAPushed(int serviceRequestId, int technicianId, string eta, string reason)
        {
            await Clients.All.SendAsync("ReceiveSmartETA", new {
                ServiceRequestId = serviceRequestId,
                TechnicianId = technicianId,
                ETA = eta,
                Reason = reason
            });
        }
    }
}
