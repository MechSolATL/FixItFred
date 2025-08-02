using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Hubs
{
    // FixItFred � Sprint 46.1 Build Stabilization
    // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Enhanced with heartbeat and connection lifecycle management
    public class JobMessageHub : Hub
    {
        private readonly ILogger<JobMessageHub> _logger;

        public JobMessageHub(ILogger<JobMessageHub> logger)
        {
            _logger = logger;
        }

        // Stub for SignalR hub used by job messaging features
        public async Task SendMessage(int requestId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", requestId, message);
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Added heartbeat ping/pong mechanism
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Connection lifecycle with diagnostics
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userAgent = Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            
            _logger.LogInformation("JobMessageHub: Client connected - ConnectionId: {ConnectionId}, UserAgent: {UserAgent}", 
                connectionId, userAgent);
            
            // Send initial heartbeat
            await Clients.Caller.SendAsync("Connected", new { 
                connectionId, 
                serverTime = DateTime.UtcNow,
                heartbeatInterval = 30000 // 30 seconds
            });
            
            await base.OnConnectedAsync();
        }

        // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Disconnection handling with cleanup
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            
            if (exception != null)
            {
                _logger.LogWarning("JobMessageHub: Client disconnected with error - ConnectionId: {ConnectionId}, Error: {Error}", 
                    connectionId, exception.Message);
            }
            else
            {
                _logger.LogInformation("JobMessageHub: Client disconnected gracefully - ConnectionId: {ConnectionId}", connectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
