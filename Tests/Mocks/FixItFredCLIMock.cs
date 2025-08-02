using Interfaces;

namespace Tests.Mocks;

/// <summary>
/// Mock implementation of IFixItFredCLI for testing diagnostic operations
/// Sprint121: Tactical add-on for DI-enabled test framework
/// </summary>
public class FixItFredCLIMock : IFixItFredCLI
{
    private bool _isHealthy = true;
    private string _diagnosticResult = "FixItFred CLI diagnostics passed - [Mock]";

    /// <summary>
    /// Simulates running diagnostic checks
    /// </summary>
    /// <returns>Mock diagnostic result</returns>
    public Task<string> RunDiagnosticsAsync()
    {
        return Task.FromResult(_diagnosticResult);
    }

    /// <summary>
    /// Simulates health check status
    /// </summary>
    /// <returns>Mock health status</returns>
    public Task<bool> IsHealthyAsync()
    {
        return Task.FromResult(_isHealthy);
    }

    /// <summary>
    /// Sets the mock health status for testing
    /// </summary>
    /// <param name="isHealthy">Health status to mock</param>
    public void SetHealthStatus(bool isHealthy)
    {
        _isHealthy = isHealthy;
    }

    /// <summary>
    /// Sets the mock diagnostic result for testing
    /// </summary>
    /// <param name="result">Diagnostic result to mock</param>
    public void SetDiagnosticResult(string result)
    {
        _diagnosticResult = result;
    }
}