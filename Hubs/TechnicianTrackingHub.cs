using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    // Sprint 91.7.Part4: SignalR hub for technician tracking
    [Authorize(Roles = "Admin,Dispatcher")]
    public class TechnicianTrackingHub : Hub
    {
        // No custom methods needed for now; use SendAsync from backend
    }
}
