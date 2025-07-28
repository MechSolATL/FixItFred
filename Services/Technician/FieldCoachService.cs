using System;
using System.Threading.Tasks;
using MVP_Core.Data;
using MVP_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Technician
{
    // Sprint 86.7 — Field Coach Service for Technician AI Companion
    public class FieldCoachService
    {
        private readonly ApplicationDbContext _db;
        public FieldCoachService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> DetectArrivalViaGeoAsync(int techId, (double lat, double lng) jobLatLng)
        {
            var tech = await _db.Technicians.FindAsync(techId);
            if (tech == null || !tech.Latitude.HasValue || !tech.Longitude.HasValue) return false;
            double dist = Math.Sqrt(Math.Pow(tech.Latitude.Value - jobLatLng.lat, 2) + Math.Pow(tech.Longitude.Value - jobLatLng.lng, 2));
            return dist < 0.001; // ~100m for demo
        }
        public async Task LogFallbackArrivalTimeIfNotSet(int techId, int jobId, DateTime fallbackTime)
        {
            var job = await _db.ServiceRequests.FindAsync(jobId);
            if (job != null && job.ArrivalTime == null)
            {
                job.ArrivalTime = fallbackTime;
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> DetectIdleAfterInvoice(int techId, int lastJobId)
        {
            var job = await _db.ServiceRequests.FindAsync(lastJobId);
            if (job == null || job.InvoiceCompletedAt == null) return false;
            var now = DateTime.UtcNow;
            return (now - job.InvoiceCompletedAt.Value).TotalMinutes > 15;
        }
        public string RecommendNextStatus(int techId)
        {
            return "Available"; // Stub: always available
        }
    }
}
