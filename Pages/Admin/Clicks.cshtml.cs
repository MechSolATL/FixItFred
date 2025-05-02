using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Admin
{
    public class ClicksModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public List<PageVisit> ClickLogs { get; set; } = [];

        public ClicksModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            ClickLogs = await _dbContext.PageVisits
                .OrderByDescending(x => x.VisitTimestamp)
                .Take(500)
                .ToListAsync();
        }
    }
}
