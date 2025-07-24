using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class ManageQuestionsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public List<Question> Questions { get; set; } = new();

        public ManageQuestionsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            Questions = await _dbContext.Questions
                .Include(q => q.Options)
                .OrderBy(q => q.ServiceType)
                .ThenBy(q => q.Id)
                .ToListAsync();
        }
    }
}
