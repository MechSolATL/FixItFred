using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Hubs
{
    // [FixItFredComment:Sprint1006 - SignalR resilience upgrade] Enhanced with heartbeat and connection lifecycle management
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task SendNotification(string message, string severity)
        {
            await Clients.All.SendAsync("ReceiveNotification", message, severity);
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
            
            _logger.LogInformation("NotificationHub: Client connected - ConnectionId: {ConnectionId}, UserAgent: {UserAgent}", 
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
                _logger.LogWarning("NotificationHub: Client disconnected with error - ConnectionId: {ConnectionId}, Error: {Error}", 
                    connectionId, exception.Message);
            }
            else
            {
                _logger.LogInformation("NotificationHub: Client disconnected gracefully - ConnectionId: {ConnectionId}", connectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
