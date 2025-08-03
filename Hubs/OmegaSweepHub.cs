using Microsoft.AspNetCore.SignalR;
using MVP_Core.Services.FixItFred;

namespace MVP_Core.Hubs;

public class OmegaSweepHub : Hub
{
    public async Task JoinSweepGroup(string sweepId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"sweep-{sweepId}");
    }

    public async Task LeaveSweepGroup(string sweepId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"sweep-{sweepId}");
    }
}

public class OmegaSweepHubClient
{
    private readonly IHubContext<OmegaSweepHub> _hubContext;

    public OmegaSweepHubClient(IHubContext<OmegaSweepHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendProgressUpdate(string sweepId, object status, string message)
    {
        await _hubContext.Clients.Group($"sweep-{sweepId}").SendAsync("ProgressUpdate", status, message);
    }

    public async Task SendRunUpdate(string sweepId, int runNumber, object runStatus)
    {
        await _hubContext.Clients.Group($"sweep-{sweepId}").SendAsync("RunUpdate", runNumber, runStatus);
    }
}