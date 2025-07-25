using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianAnalyticsModel : PageModel
    {
        private readonly TechnicianProfileService _profileService;
        private readonly ITechnicianService _techService;
        public List<MVP_Core.Data.Models.Technician> Technicians { get; set; } = new();
        public List<int> SelectedTechIds { get; set; } = new();
        public Dictionary<int, TechnicianAnalyticsDto> Analytics { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TechnicianAnalyticsModel(TechnicianProfileService profileService, ITechnicianService techService)
        {
            _profileService = profileService;
            _techService = techService;
        }

        public async Task OnGetAsync(string? techIds = null, string? start = null, string? end = null)
        {
            StartDate = string.IsNullOrEmpty(start) ? DateTime.UtcNow.AddDays(-30) : DateTime.Parse(start);
            EndDate = string.IsNullOrEmpty(end) ? DateTime.UtcNow : DateTime.Parse(end);
            // Load all techs for dropdown
            var techViewModels = await _techService.GetAllAsync();
            Technicians = techViewModels.Select(t => new MVP_Core.Data.Models.Technician
            {
                Id = t.Id,
                FullName = t.FullName,
                IsActive = t.IsActive,
                Email = t.Email,
                Phone = t.Phone,
                Specialty = t.Specialty
            }).OrderBy(t => t.FullName).ToList();
            // Parse selected techs
            SelectedTechIds = string.IsNullOrEmpty(techIds)
                ? (Technicians.Count > 0 ? new List<int> { Technicians[0].Id } : new List<int>())
                : techIds.Split(',').Select(int.Parse).ToList();
            // Load analytics for selected techs
            Analytics.Clear();
            foreach (var id in SelectedTechIds)
            {
                var dto = await _profileService.GetAnalyticsAsync(id, new DateRange { Start = StartDate, End = EndDate });
                Analytics[id] = dto;
            }
        }
    }
}
