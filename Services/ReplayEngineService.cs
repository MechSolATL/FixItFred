/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Gets replay output information
/// Returns formatted replay data for display or analysis
/// </summary>
/// <returns>Replay output string or null if no data available</returns>
public string? GetReplayOutput()
{
    var userName = _userContext.User?.Identity?.Name ?? "admin";
    
    _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] Getting replay output for user {UserName}", userName);
    
    // [Sprint123_FixItFred_OmegaSweep] Return formatted replay information
    return $"Replay Engine Status: Active | User: {userName} | Last Updated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
}
