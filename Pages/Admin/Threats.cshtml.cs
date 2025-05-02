using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class ThreatsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ThreatsModel> _logger;

        public PaginatedList<ThreatBlock> Threats { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        private const int PageSize = 20;

        public ThreatsModel(ApplicationDbContext dbContext, ILogger<ThreatsModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var query = _dbContext.ThreatBlocks
                    .OrderByDescending(t => t.LastDetectedAt)
                    .AsQueryable();

                Threats = await PaginatedList<ThreatBlock>.CreateAsync(query, PageIndex, PageSize);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading threat blocks.");
                ModelState.AddModelError(string.Empty, "Failed to load threat data.");
                Threats = new PaginatedList<ThreatBlock>(new List<ThreatBlock>(), 0, PageIndex, PageSize);
                return Page();
            }
        }
    }
}
