using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Hubs
{
    // Sprint 91.7.Part4: SignalR hub for technician tracking
    // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Enhanced with heartbeat and connection lifecycle management
    [Authorize(Roles = "Admin,Dispatcher")]
    public class TechnicianTrackingHub : Hub
    {
        private readonly ILogger<TechnicianTrackingHub> _logger;

        public TechnicianTrackingHub(ILogger<TechnicianTrackingHub> logger)
        {
            _logger = logger;
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Added heartbeat ping/pong mechanism
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Connection lifecycle with role-based diagnostics
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userAgent = Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            
            _logger.LogInformation("TechnicianTrackingHub: Authorized user connected - ConnectionId: {ConnectionId}, User: {UserName}, UserAgent: {UserAgent}", 
                connectionId, userName, userAgent);
            
            // Send initial heartbeat
            await Clients.Caller.SendAsync("Connected", new { 
                connectionId, 
                userName,
                serverTime = DateTime.UtcNow,
                heartbeatInterval = 30000 // 30 seconds
            });
            
            await base.OnConnectedAsync();
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Disconnection handling with user context
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            
            if (exception != null)
            {
                _logger.LogWarning("TechnicianTrackingHub: User disconnected with error - ConnectionId: {ConnectionId}, User: {UserName}, Error: {Error}", 
                    connectionId, userName, exception.Message);
            }
            else
            {
                _logger.LogInformation("TechnicianTrackingHub: User disconnected gracefully - ConnectionId: {ConnectionId}, User: {UserName}", 
                    connectionId, userName);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // No custom methods needed for now; use SendAsync from backend
    }
}
