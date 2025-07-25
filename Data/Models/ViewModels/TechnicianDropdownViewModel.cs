// FixItFred Patch Log — Sprint 28: Validation Engine
// [2024-07-25T00:35:00Z] — Removed duplicate TechnicianDropdownViewModel for build compliance.
using System.ComponentModel.DataAnnotations;
namespace MVP_Core.Data.Models.ViewModels
{
    public class TechnicianDropdownViewModel
    {
        [Required]
        public List<MVP_Core.Models.Admin.TechnicianProfileDto> Technicians { get; set; } = new();
        public int? SelectedTechnicianId { get; set; }
    }
}
