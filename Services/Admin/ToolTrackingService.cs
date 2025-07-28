// Sprint 91.7 Part 6.2 — ToolTrackingService for managing technician tools
using MVP_Core.Data;
using MVP_Core.Data.DTO.Tools;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
{
    public class ToolTrackingService
    {
        private readonly ApplicationDbContext _context;

        public ToolTrackingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ToolDto>> GetAllToolsAsync()
        {
            return await _context.ToolInventories
                .Include(t => t.AssignedTechnician)
                .Select(t => new ToolDto
                {
                    ToolId = new Guid(t.ToolId.ToString()), // Convert int ToolId to Guid for DTO compatibility
                    Name = t.Name,
                    ToolType = t.ConditionStatus, // No ToolType in model, using ConditionStatus as placeholder
                    Status = t.IsActive ? (t.AssignedTechId != null ? "InUse" : "Available") : "Inactive",
                    AssignedTechnicianId = t.AssignedTechId != null ? new Guid(t.AssignedTechId.Value.ToString()) : (Guid?)null,
                    AssignedTechnicianName = t.AssignedTechnician != null ? t.AssignedTechnician.FullName : null,
                    LastTransferDate = null, // Not tracked in ToolInventory, could be added
                    IsTransferable = t.IsActive
                }).ToListAsync();
        }

        public async Task<bool> AssignToolAsync(int toolId, int technicianId)
        {
            var tool = await _context.ToolInventories.FindAsync(toolId);
            if (tool == null || !tool.IsActive) return false;

            tool.AssignedTechId = technicianId;
            // No LastTransferDate in model, could be added
            _context.ToolTransferLogs.Add(new ToolTransferLog
            {
                ToolId = toolId,
                FromTechId = 0, // 0 = system/initial assignment
                ToTechId = technicianId,
                Timestamp = DateTime.UtcNow,
                Notes = "Initial Assignment",
                ConfirmedByReceiver = false
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferToolAsync(int toolId, int fromTechId, int toTechId, string? notes = null)
        {
            var tool = await _context.ToolInventories.FindAsync(toolId);
            if (tool == null || !tool.IsActive) return false;
            if (tool.AssignedTechId != fromTechId) return false;

            tool.AssignedTechId = toTechId;
            _context.ToolTransferLogs.Add(new ToolTransferLog
            {
                ToolId = toolId,
                FromTechId = fromTechId,
                ToTechId = toTechId,
                Timestamp = DateTime.UtcNow,
                Notes = notes,
                ConfirmedByReceiver = false
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ToolTransferLog>> GetTransferHistoryAsync(int toolId)
        {
            return await _context.ToolTransferLogs
                .Where(log => log.ToolId == toolId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }
}
