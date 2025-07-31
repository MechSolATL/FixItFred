using System.Threading.Tasks;
using ViewModels;

namespace Services.Diagnostics
{
    // Sprint 89.2: Interface for subsystem health checks
    public interface ISubsystemChecker
    {
        Task<ProcessStatusViewModel> CheckAsync(string subsystemName);
    }
}
