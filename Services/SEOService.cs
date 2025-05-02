using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services
{
    public interface ISeoService
    {
        Task<SEO?> GetSeoByPageNameAsync(string pageName);
    }

    public class SeoService : ISeoService
    {
        private readonly ApplicationDbContext _dbContext;

        public SeoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SEO?> GetSeoByPageNameAsync(string pageName)
        {
            if (string.IsNullOrWhiteSpace(pageName))
                return null;

            return await _dbContext.SEOs
                .AsNoTracking()
                .FirstOrDefaultAsync(seo => seo.PageName.ToLower() == pageName.ToLower());
        }
    }
}
