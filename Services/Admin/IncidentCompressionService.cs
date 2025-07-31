using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class IncidentCompressionService
    {
        private readonly ApplicationDbContext _db;
        public IncidentCompressionService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<IncidentCompressionLog>> GetIncidentClustersAsync()
        {
            return await _db.IncidentCompressionLogs
                .OrderByDescending(i => i.Timestamp)
                .ToListAsync();
        }
        public async Task AddIncidentClusterAsync(string clusterKey, int occurrenceCount, string? equipmentFaults, string? dispatchIssues, bool techBurnoutSuspected, bool clientAbuseSuspected, string? notes)
        {
            var entry = new IncidentCompressionLog
            {
                ClusterKey = clusterKey,
                OccurrenceCount = occurrenceCount,
                EquipmentFaults = equipmentFaults,
                DispatchIssues = dispatchIssues,
                TechBurnoutSuspected = techBurnoutSuspected,
                ClientAbuseSuspected = clientAbuseSuspected,
                Notes = notes,
                Timestamp = DateTime.UtcNow
            };
            _db.IncidentCompressionLogs.Add(entry);
            await _db.SaveChangesAsync();
        }
    }
}
