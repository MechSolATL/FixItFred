namespace MVP_Core.Services.LiveTracking.Models
{
    public class TechnicianLocation
    {
        public Guid TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime EstimatedArrival { get; set; }
    }
}