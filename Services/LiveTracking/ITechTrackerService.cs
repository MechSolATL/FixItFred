using MVP_Core.Services.LiveTracking.Models;

namespace MVP_Core.Services.LiveTracking
{
    public interface ITechTrackerService
    {
        TechnicianLocation? GetTechnicianLocation(Guid technicianId);
    }
}