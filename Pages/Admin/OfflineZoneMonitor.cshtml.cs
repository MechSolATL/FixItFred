using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class OfflineZoneMonitorModel : PageModel
    {
        private readonly OfflineZoneTracker _zoneTracker;
        private readonly AutoPrepEngine _prepEngine;
        public OfflineZoneMonitorModel(OfflineZoneTracker zoneTracker, AutoPrepEngine prepEngine)
        {
            _zoneTracker = zoneTracker;
            _prepEngine = prepEngine;
        }
        public List<OfflineZoneHeatmap> Zones { get; private set; } = new();
        public void OnGet()
        {
            Zones = _zoneTracker.GetAllZones().ToList();
        }
        public void OnPostPrepZone(int zoneId)
        {
            var zone = _zoneTracker.GetAllZones().FirstOrDefault(z => z.Id == zoneId);
            if (zone != null)
            {
                _prepEngine.PreloadDataForZone(zone.TechnicianId, zone.ZipCode, zone.Latitude, zone.Longitude);
            }
            Zones = _zoneTracker.GetAllZones().ToList();
        }
    }
}
