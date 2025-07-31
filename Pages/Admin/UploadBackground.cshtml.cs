using Data;
using Data.Models;

namespace Pages.Admin
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

            using MemoryStream memoryStream = new();
            await BackgroundImage.CopyToAsync(memoryStream);

            BackgroundImage image = new()
            {
                ImageData = memoryStream.ToArray(),
                ContentType = BackgroundImage.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            _ = _context.BackgroundImages.Add(image);
            _ = _context.SaveChanges();

            TempData["Success"] = "? Background image uploaded successfully!";
            return RedirectToPage("/Admin/UploadBackground");
        }
    }
}
