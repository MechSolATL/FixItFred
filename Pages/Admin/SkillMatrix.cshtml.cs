using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Data;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class SkillMatrixModel : PageModel
    {
        private readonly TechnicianSkillMatrixService _skillMatrixService;
        private readonly ApplicationDbContext _db;
        public SkillMatrixModel(TechnicianSkillMatrixService skillMatrixService, ApplicationDbContext db)
        {
            _skillMatrixService = skillMatrixService;
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int TechnicianId { get; set; }
        public List<Data.Models.Technician> Technicians { get; set; } = new List<Data.Models.Technician>();
        [BindProperty]
        public List<TechnicianSkillMatrix> SkillMatrix { get; set; } = new List<TechnicianSkillMatrix>();

        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.Where(t => t.IsActive).OrderBy(t => t.FullName).ToList();
            if (TechnicianId > 0)
            {
                SkillMatrix = await _skillMatrixService.GetSkillsByTechnicianAsync(TechnicianId);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SkillMatrix != null)
            {
                foreach (var entry in SkillMatrix)
                {
                    await _skillMatrixService.UpdateSkillMatrixAsync(entry);
                }
            }
            return RedirectToPage(new { TechnicianId });
        }
    }
}
