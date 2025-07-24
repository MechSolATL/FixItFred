// RequestHub.cs
// SignalR hub to broadcast new or reassigned service requests to connected dispatcher clients

using Microsoft.AspNetCore.SignalR;

namespace MVP_Core.Hubs
{
    public class RequestHub : Hub
    {
        // Future: support group joins or role-targeted dispatching
        // e.g., await Groups.AddToGroupAsync(Context.ConnectionId, "Dispatchers");
    }
}
