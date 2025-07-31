using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ViewModels;

namespace Services.Diagnostics
{
    // Sprint 89.2: Live polling and mock subsystem checks
    public class LiveSubsystemStatusService : ISubsystemChecker
    {
        private static readonly Dictionary<string, Func<Task<ProcessStatusViewModel>>> _mockHandlers = new()
        {
            { "Auth", async () => await MockCheck("Auth", 80, ProcessHealthStatus.Healthy) },
            { "Email", async () => await MockCheck("Email", 120, ProcessHealthStatus.Warning) },
            { "Queue", async () => await MockCheck("Queue", 200, ProcessHealthStatus.Critical) },
            { "DB", async () => await MockCheck("DB", 60, ProcessHealthStatus.Healthy) },
            { "Payments", async () => await MockCheck("Payments", 150, ProcessHealthStatus.Warning) },
            { "Forecasting", async () => await MockCheck("Forecasting", 90, ProcessHealthStatus.Healthy) },
            { "Media", async () => await MockCheck("Media", 110, ProcessHealthStatus.Healthy) },
        };

        public async Task<ProcessStatusViewModel> CheckAsync(string subsystemName)
        {
            if (_mockHandlers.TryGetValue(subsystemName, out var handler))
                return await handler();
            // Default unknown
            return new ProcessStatusViewModel
            {
                Name = subsystemName,
                Status = ProcessHealthStatus.Unknown,
                LastChecked = DateTime.UtcNow,
                PingLatencyMs = 0,
                SuggestedAction = "No handler registered."
            };
        }

        private static async Task<ProcessStatusViewModel> MockCheck(string name, int latency, ProcessHealthStatus status)
        {
            var sw = Stopwatch.StartNew();
            await Task.Delay(latency); // Simulate ping
            sw.Stop();
            return new ProcessStatusViewModel
            {
                Name = name,
                Status = status,
                LastChecked = DateTime.UtcNow,
                PingLatencyMs = (int)sw.ElapsedMilliseconds,
                SuggestedAction = status switch
                {
                    ProcessHealthStatus.Healthy => "No action needed",
                    ProcessHealthStatus.Warning => "Check configuration or logs",
                    ProcessHealthStatus.Critical => "Immediate attention required",
                    _ => "Unknown status"
                }
            };
        }
    }
}
