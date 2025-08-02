namespace Interfaces;

/// <summary>
/// Interface for FixItFred CLI diagnostic operations
/// </summary>
public interface IFixItFredCLI
{
    /// <summary>
    /// Performs diagnostic check
    /// </summary>
    /// <returns>Diagnostic result</returns>
    Task<string> RunDiagnosticsAsync();
    
    /// <summary>
    /// Gets FixItFred status
    /// </summary>
    /// <returns>Status information</returns>
    Task<bool> IsHealthyAsync();
}