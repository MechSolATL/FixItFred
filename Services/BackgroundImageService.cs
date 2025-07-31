using Data;
using Data.Models;

namespace Services
{
    public class BackgroundImageService
    {
        private readonly ApplicationDbContext _context;

        public BackgroundImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BackgroundImage?> GetLatestBackgroundImageAsync()
        {
            return await _context.BackgroundImages
                .OrderByDescending(static b => b.UploadedAt)
                .FirstOrDefaultAsync();
        }
    }
}
