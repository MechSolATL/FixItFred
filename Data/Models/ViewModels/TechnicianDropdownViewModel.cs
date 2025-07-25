// FixItFred Patch Log — Sprint 28: Validation Engine
// [2024-07-25T00:35:00Z] — Removed duplicate TechnicianDropdownViewModel for build compliance.
// FixItFred — Sprint 46.1 Build Correction + Compliance
using System.ComponentModel.DataAnnotations;
using MVP_Core.Models.Admin;

namespace MVP_Core.Data.Models.ViewModels
{
    public class TechnicianDropdownViewModel
    {
        [Required]
        // FixItFred — Sprint 46.1 Build Correction + Compliance: Use TechnicianStatusDto for dropdown
        public List<TechnicianStatusDto> Technicians { get; set; } = new();
        public int? SelectedTechnicianId { get; set; }
    }
}
