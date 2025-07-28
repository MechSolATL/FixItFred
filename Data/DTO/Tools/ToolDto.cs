// Sprint 91.7 Part 6.1 — DTO Definition for Inventory Tool
namespace MVP_Core.Data.DTO.Tools
{
    public class ToolDto
    {
        public Guid ToolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; // Available, InUse, Damaged, Lost
        public Guid? AssignedTechnicianId { get; set; }
        public string? AssignedTechnicianName { get; set; }
        public DateTime? LastTransferDate { get; set; }
        public bool IsTransferable { get; set; } = true;
    }
}
