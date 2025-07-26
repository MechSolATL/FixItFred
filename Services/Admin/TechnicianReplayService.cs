using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class TechnicianReplayService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianReplayService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TechnicianIncidentReplay>> GetReplaysByTechnicianAsync(int technicianId)
        {
            return await _db.TechnicianIncidentReplays
                .Where(r => r.TechnicianId == technicianId)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }

        public async Task AddIncidentReplayAsync(TechnicianIncidentReplay entry)
        {
            _db.TechnicianIncidentReplays.Add(entry);
            await _db.SaveChangesAsync();
        }
    }
}
