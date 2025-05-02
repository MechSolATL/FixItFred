using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Pages.Admin
{
    public class UploadBackgroundModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UploadBackgroundModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile? BackgroundImage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (BackgroundImage == null || BackgroundImage.Length == 0)
            {
                ModelState.AddModelError("BackgroundImage", "Please select a valid image file.");
                return Page();
            }

            using var memoryStream = new MemoryStream();
            await BackgroundImage.CopyToAsync(memoryStream);

            var image = new BackgroundImage
            {
                ImageData = memoryStream.ToArray(),
                ContentType = BackgroundImage.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            _context.BackgroundImages.Add(image);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Background image uploaded successfully!";
            return RedirectToPage("/Admin/UploadBackground");
        }
    }
}
