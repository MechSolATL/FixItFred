using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace Services.Diagnostics
{
    public class ProcessMonitorService
    {
        private readonly ISubsystemChecker _subsystemChecker;
        public ProcessMonitorService(ISubsystemChecker subsystemChecker)
        {
            _subsystemChecker = subsystemChecker;
        }

        // Sprint 89.2: Now async and uses subsystem checker
        public async Task<List<ProcessStatusViewModel>> GetAllProcessStatusesAsync()
        {
            var subsystems = new[] { "Auth", "Email", "Queue", "DB", "Payments", "Forecasting", "Media" };
            var results = new List<ProcessStatusViewModel>();
            foreach (var name in subsystems)
            {
                results.Add(await _subsystemChecker.CheckAsync(name));
            }
            return results;
        }
    }
}
