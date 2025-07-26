using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
{
    public class TechnicianEscalationService
    {
        private readonly ApplicationDbContext _context;
        public TechnicianEscalationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogEscalationAsync(int technicianId, int escalationLevel, string escalationReason)
        {
            var log = new TechnicianEscalationLog
            {
                TechnicianId = technicianId,
                EscalationLevel = escalationLevel,
                EscalationReason = escalationReason,
                Resolved = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.TechnicianEscalationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TechnicianEscalationLog>> GetEscalationsAsync(int? technicianId = null, bool? resolved = null)
        {
            var query = _context.TechnicianEscalationLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(e => e.TechnicianId == technicianId.Value);
            if (resolved.HasValue)
                query = query.Where(e => e.Resolved == resolved.Value);
            return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
        }

        public async Task<bool> ResolveEscalationAsync(int escalationId, string resolutionNotes, int? trustImpactTier = null)
        {
            var escalation = await _context.TechnicianEscalationLogs.FindAsync(escalationId);
            if (escalation == null) return false;
            escalation.Resolved = true;
            escalation.ResolvedAt = DateTime.UtcNow;
            escalation.ResolutionNotes = resolutionNotes;
            // TODO: Inject trust impact logic if trustImpactTier is provided
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
