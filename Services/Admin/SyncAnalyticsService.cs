using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Admin
{
    public class SyncAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public SyncAnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<TechnicianSyncMetricsDto> GetTechnicianSyncMetrics()
        {
            // Aggregate sync metrics from TechnicianOfflineSession and MediaQueueHandler logs
            var sessions = _db.TechnicianOfflineSessions.ToList();
            var mediaLogs = _db.MediaSyncLogs.ToList(); // Assume MediaSyncLogs table exists for upload attempts
            var techs = _db.Technicians.ToList();
            var metrics = new List<TechnicianSyncMetricsDto>();
            foreach (var tech in techs)
            {
                var techMedia = mediaLogs.Where(m => m.TechnicianId == tech.Id);
                var successCount = techMedia.Count(m => m.IsSuccess);
                var totalCount = techMedia.Count();
                var avgRetry = techMedia.Any() ? techMedia.Average(m => m.RetryCount) : 0;
                var offlineRecovered = techMedia.Count(m => m.TriggeredFromOffline);
                metrics.Add(new TechnicianSyncMetricsDto
                {
                    TechnicianId = tech.Id,
                    TechnicianName = tech.Name,
                    SuccessPercent = totalCount > 0 ? (int)(100.0 * successCount / totalCount) : 0,
                    AvgRetryCount = (int)Math.Round(avgRetry),
                    TotalFailedRecovered = offlineRecovered
                });
            }
            return metrics;
        }

        public List<SyncZoneHeatmapDto> GetSyncZoneHeatmap()
        {
            // Correlate upload delays with OfflineZoneHeatmap
            var heatmaps = _db.OfflineZoneHeatmaps.ToList();
            var mediaLogs = _db.MediaSyncLogs.ToList();
            var zoneMetrics = new List<SyncZoneHeatmapDto>();
            foreach (var zone in heatmaps)
            {
                var zoneMedia = mediaLogs.Where(m => m.Zone == zone.Zone);
                zoneMetrics.Add(new SyncZoneHeatmapDto
                {
                    Zone = zone.Zone,
                    DelayedUploads = zoneMedia.Count(m => m.RetryCount > 0),
                    AvgRetry = zoneMedia.Any() ? zoneMedia.Average(m => m.RetryCount) : 0,
                    PoorSignalScore = zone.PoorSignalScore
                });
            }
            return zoneMetrics;
        }
    }

    public class TechnicianSyncMetricsDto
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = "";
        public int SuccessPercent { get; set; }
        public int AvgRetryCount { get; set; }
        public int TotalFailedRecovered { get; set; }
    }

    public class SyncZoneHeatmapDto
    {
        public string Zone { get; set; } = "";
        public int DelayedUploads { get; set; }
        public double AvgRetry { get; set; }
        public int PoorSignalScore { get; set; }
    }
}
