// Sprint 91.7 Part 6.1 — DTO Definition for Tool Transfers
namespace MVP_Core.Data.DTO.Tools
{
    public class ToolTransferRequestDto
    {
        public Guid ToolId { get; set; }
        public Guid FromTechnicianId { get; set; }
        public Guid ToTechnicianId { get; set; }
        public DateTime TransferInitiatedAt { get; set; } = DateTime.UtcNow;
        public string? TransferNotes { get; set; }
    }
}
