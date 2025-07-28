using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models.NovaIntel;

namespace MVP_Core.Services.Nova
{
    public class NovaIntelMemoryService
    {
        private readonly ApplicationDbContext _context;

        public NovaIntelMemoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogDecisionAsync(NovaDecisionMemory memory)
        {
            _context.NovaDecisionMemories.Add(memory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NovaDecisionMemory>> GetHistoryForTenantAsync(Guid tenantId)
        {
            return await _context.NovaDecisionMemories
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
