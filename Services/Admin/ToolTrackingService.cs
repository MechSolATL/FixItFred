// Sprint 91.7 Part 6.2 — ToolTrackingService for managing technician tools
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Hubs;
using Data.DTO.Tools;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class ToolTrackingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ToolTrackingHub> _hubContext;

        public ToolTrackingService(ApplicationDbContext context, IHubContext<ToolTrackingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Get all tools with assignment and transferability info
        public async Task<List<ToolDto>> GetAllToolsAsync()
        {
            return await _context.ToolInventories
                .Include(t => t.AssignedTechnician)
                .Select(t => new ToolDto
                {
                    ToolId = GuidFromInt(t.ToolId),
                    Name = t.Name,
                    ToolType = t.ConditionStatus, // No ToolType in model, using ConditionStatus as placeholder
                    Status = t.IsActive ? t.AssignedTechId != null ? "InUse" : "Available" : "Inactive",
                    AssignedTechnicianId = t.AssignedTechId != null ? GuidFromInt(t.AssignedTechId.Value) : null,
                    AssignedTechnicianName = t.AssignedTechnician != null ? t.AssignedTechnician.FullName : null,
                    LastTransferDate = GetLastTransferDate(t.ToolId),
                    IsTransferable = t.IsActive
                }).ToListAsync();
        }

        // Assign a tool to a technician (initial assignment)
        public async Task<bool> AssignToolAsync(Guid toolId, Guid technicianId)
        {
            int toolIntId = IntFromGuid(toolId);
            int techIntId = IntFromGuid(technicianId);
            var tool = await _context.ToolInventories.FindAsync(toolIntId);
            if (tool == null || !tool.IsActive) return false;

            tool.AssignedTechId = techIntId;
            // Log assignment
            _context.ToolTransferLogs.Add(new ToolTransferLog
            {
                ToolId = toolIntId,
                FromTechId = 0, // 0 = system/initial assignment
                ToTechId = techIntId,
                Timestamp = DateTime.UtcNow,
                Notes = "Initial Assignment",
                ConfirmedByReceiver = false
            });

            await _context.SaveChangesAsync();
            // Broadcast assignment event
            await _hubContext.Clients.All.SendAsync("toolAssigned", new { ToolId = toolId, TechnicianId = technicianId });
            return true;
        }

        // Transfer a tool between technicians
        public async Task<bool> TransferToolAsync(ToolTransferRequestDto dto)
        {
            int toolIntId = IntFromGuid(dto.ToolId);
            int fromTechIntId = IntFromGuid(dto.FromTechnicianId);
            int toTechIntId = IntFromGuid(dto.ToTechnicianId);
            var tool = await _context.ToolInventories.FindAsync(toolIntId);
            if (tool == null || !tool.IsActive) {
                await _hubContext.Clients.All.SendAsync("toolTransferFailed", new { dto.ToolId, Reason = "Tool not found or inactive" });
                return false;
            }
            if (tool.AssignedTechId != fromTechIntId) {
                await _hubContext.Clients.All.SendAsync("toolTransferFailed", new { dto.ToolId, Reason = "Tool not assigned to this technician" });
                return false;
            }

            tool.AssignedTechId = toTechIntId;
            // Log transfer
            _context.ToolTransferLogs.Add(new ToolTransferLog
            {
                ToolId = toolIntId,
                FromTechId = fromTechIntId,
                ToTechId = toTechIntId,
                Timestamp = dto.TransferInitiatedAt,
                Notes = dto.TransferNotes,
                ConfirmedByReceiver = false
            });

            await _context.SaveChangesAsync();
            // Broadcast transfer event
            await _hubContext.Clients.All.SendAsync("toolConfirmed", new { dto.ToolId, dto.FromTechnicianId, dto.ToTechnicianId });
            return true;
        }

        // Get transfer history for a tool
        public async Task<List<ToolTransferLog>> GetTransferHistoryAsync(Guid toolId)
        {
            int toolIntId = IntFromGuid(toolId);
            return await _context.ToolTransferLogs
                .Where(log => log.ToolId == toolIntId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        // Sprint 91.7 Part 6.3: Get all active technicians for transfer dropdown
        public async Task<List<Data.Models.Technician>> GetAvailableTechsAsync()
        {
            return await _context.Technicians.Where(t => t.IsActive).OrderBy(t => t.FullName).ToListAsync();
        }

        // --- Helper methods for Guid <-> int mapping (for legacy int PKs) ---
        private static Guid GuidFromInt(int id)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(id).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
        private static int IntFromGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();
            return BitConverter.ToInt32(bytes, 0);
        }
        private DateTime? GetLastTransferDate(int toolId)
        {
            var lastLog = _context.ToolTransferLogs
                .Where(l => l.ToolId == toolId)
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();
            return lastLog?.Timestamp;
        }
    }
}
