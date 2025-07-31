namespace Services.Simulated
{
    public class ValidationSimulatorService
    {
        public Task<bool> RunSimulatedDiagnosticsAsync(string technicianId, string region) =>
            Task.FromResult(true);

        public Task TriggerSyntheticAlertAsync(string alertType, string? notes = null) =>
            Task.CompletedTask;

        public Task<string> GenerateMockReportAsync(string region)
        {
            return Task.FromResult($"Report for region {region} generated at {DateTime.Now}");
        }

        public Task<int> EvaluateRiskScoreAsync(string technicianId)
        {
            return Task.FromResult(75); // mock score
        }
    }
}
