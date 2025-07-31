using MVP_Core.Services.LiveTracking.Models;

namespace MVP_Core.Services.LiveTracking
{
    public class TechTrackerService : ITechTrackerService
    {
        public TechnicianLocation? GetTechnicianLocation(Guid technicianId)
        {
            return new TechnicianLocation
            {
                TechnicianId = technicianId,
                Name = "Alex J.",
                Latitude = 33.7490,
                Longitude = -84.3880,
                EstimatedArrival = DateTime.UtcNow.AddMinutes(17)
            };
        }
    }
}