using System;
using System.Collections.Generic;
using TechnicianModel = MVP_Core.Data.Models.Technician;

namespace MVP_Core.Services.Dispatcher
{
    // Pin for map display
    public class TechMapPin
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TechMapService
    {
        private readonly ApplicationDbContext _db;
        public TechMapService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Returns latest GPS for all active techs
        public List<TechMapPin> GetActiveTechnicianPins()
        {
            var pins = new List<TechMapPin>();
            var techs = _db.Technicians.Where(t => t.IsActive && t.Latitude != null && t.Longitude != null).ToList();
            foreach (var t in techs)
            {
                pins.Add(new TechMapPin
                {
                    TechnicianId = t.Id,
                    Name = t.FullName,
                    Latitude = t.Latitude ?? 0,
                    Longitude = t.Longitude ?? 0,
                    Status = t.CurrentJobCount == 0 ? "Idle" : (t.CurrentJobCount < t.MaxJobCapacity ? "Working" : "Busy")
                });
            }
            return pins;
        }
    }
}
