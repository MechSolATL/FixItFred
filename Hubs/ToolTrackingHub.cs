using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MVP_Core.Hubs
{
    // Sprint 91.7.Part6.4: SignalR hub for tool tracking and transfer events
    [Authorize(Roles = "Admin,Dispatcher,Manager")]
    public class ToolTrackingHub : Hub
    {
        // No custom methods needed for now; use SendAsync from backend
    }
}
