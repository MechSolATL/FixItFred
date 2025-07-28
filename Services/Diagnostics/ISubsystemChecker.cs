using System.Threading.Tasks;
using MVP_Core.ViewModels;

namespace MVP_Core.Services.Diagnostics
{
    // Sprint 89.2: Interface for subsystem health checks
    public interface ISubsystemChecker
    {
        Task<ProcessStatusViewModel> CheckAsync(string subsystemName);
    }
}
