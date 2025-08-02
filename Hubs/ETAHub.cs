// FixItFred Patch Log � Sprint 29B: Hardened ETAHub Security
// [2025-07-25T00:00:00Z] � Explicitly marked ETAHub as [AllowAnonymous] for demo; restrict as needed for production.
// [2025-07-25T00:00:00Z] � Added zone-based SignalR group routing and OnConnectedAsync group join logic.
// [2025-09-10T00:00:00Z] � Patched ETAHub to use string zone for group routing and message broadcast. FixItFred Sprint 30D.1.
// [2026-03-22T00:00:00Z] � Added UpdateTechnicianLocation method for live marker updates. FixItFred Sprint 33.3.
// [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Enhanced with heartbeat and improved connection lifecycle management
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Hubs
{
    [AllowAnonymous] // FixItFred: Set to [Authorize] for production security
    public class ETAHub : Hub
    {
        private readonly ILogger<ETAHub> _logger;

        public ETAHub(ILogger<ETAHub> logger)
        {
            _logger = logger;
        }

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

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Added heartbeat ping/pong mechanism
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Enhanced connection handling with improved diagnostics
        public override async Task OnConnectedAsync()
        {
            // FixItFred: Join group based on querystring zone (default "1")
            var httpContext = Context.GetHttpContext();
            var zone = httpContext?.Request.Query["zone"];
            if (string.IsNullOrEmpty(zone)) zone = "1";
            
            var connectionId = Context.ConnectionId;
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            
            await Groups.AddToGroupAsync(connectionId, $"Zone-{zone}");
            
            _logger.LogInformation("ETAHub: Client connected to Zone-{Zone} - ConnectionId: {ConnectionId}, UserAgent: {UserAgent}", 
                zone, connectionId, userAgent);
            
            // Send initial heartbeat with zone info
            await Clients.Caller.SendAsync("Connected", new { 
                connectionId, 
                zone,
                serverTime = DateTime.UtcNow,
                heartbeatInterval = 30000 // 30 seconds
            });
            
            await base.OnConnectedAsync();
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Disconnection handling with zone cleanup
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var httpContext = Context.GetHttpContext();
            var zone = httpContext?.Request.Query["zone"];
            if (string.IsNullOrEmpty(zone)) zone = "1";
            
            if (exception != null)
            {
                _logger.LogWarning("ETAHub: Client disconnected from Zone-{Zone} with error - ConnectionId: {ConnectionId}, Error: {Error}", 
                    zone, connectionId, exception.Message);
            }
            else
            {
                _logger.LogInformation("ETAHub: Client disconnected gracefully from Zone-{Zone} - ConnectionId: {ConnectionId}", 
                    zone, connectionId);
            }
            
            // Group cleanup is automatic, but we log for diagnostics
            await base.OnDisconnectedAsync(exception);
        }
        // Sprint 33.3 - SignalR Integration
        // Called when a technician's GPS or job status changes
        public async Task UpdateTechnicianLocation(int technicianId, double lat, double lng, string status, int jobs, string eta, string currentJob, string name)
        {
            // Broadcast to all dispatcher clients (or by zone if needed)
            await Clients.All.SendAsync("UpdateTechnicianLocation", new {
                id = technicianId,
                name,
                lat,
                lng,
                status,
                jobs,
                eta,
                currentJob
            });
        }
    }
}
