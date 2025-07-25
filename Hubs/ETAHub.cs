// FixItFred Patch Log — Sprint 29B: Hardened ETAHub Security
// [2025-07-25T00:00:00Z] — Explicitly marked ETAHub as [AllowAnonymous] for demo; restrict as needed for production.
// [2025-07-25T00:00:00Z] — Added zone-based SignalR group routing and OnConnectedAsync group join logic.
// [2025-09-10T00:00:00Z] — Patched ETAHub to use string zone for group routing and message broadcast. FixItFred Sprint 30D.1.
// [2026-03-22T00:00:00Z] — Added UpdateTechnicianLocation method for live marker updates. FixItFred Sprint 33.3.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MVP_Core.Hubs
{
    [AllowAnonymous] // FixItFred: Set to [Authorize] for production security
    public class ETAHub : Hub
    {
        // FixItFred: Sprint 30D.1 - Live ETA Routing & Broadcast
        // Receives ETA updates and broadcasts to SignalR groups (Zone-{zoneId})
        public async Task BroadcastETA(int technicianId, string eta)
        {
            await Clients.All.SendAsync("ReceiveETA", technicianId, eta);
        }
        public async Task SendETA(string zone, string message)
        {
            await Clients.Group($"Zone-{zone}").SendAsync("ReceiveETA", zone, message);
        }
        public override async Task OnConnectedAsync()
        {
            // FixItFred: Join group based on querystring zone (default "1")
            var httpContext = Context.GetHttpContext();
            var zone = httpContext?.Request.Query["zone"];
            if (string.IsNullOrEmpty(zone)) zone = "1";
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Zone-{zone}");
            await base.OnConnectedAsync();
        }
        // Sprint 33.3 - SignalR Integration
        // Called when a technician's GPS or job status changes
        public async Task UpdateTechnicianLocation(int technicianId, double lat, double lng, string status, int jobs, string eta, string currentJob, string name)
        {
            // Broadcast to all dispatcher clients (or by zone if needed)
            await Clients.All.SendAsync("UpdateTechnicianLocation", new {
                id = technicianId,
                name = name,
                lat = lat,
                lng = lng,
                status = status,
                jobs = jobs,
                eta = eta,
                currentJob = currentJob
            });
        }
    }
}
