using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services
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
                .OrderByDescending(b => b.UploadedAt)
                .FirstOrDefaultAsync();
        }
    }
}
