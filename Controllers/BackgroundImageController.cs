using MVP_Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;

namespace MVP_Core.Controllers
{
    [Route("stream")]
    public class BackgroundImageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BackgroundImageController> _logger;

        public BackgroundImageController(ApplicationDbContext context, ILogger<BackgroundImageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("background")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client, NoStore = false, VaryByHeader = "User-Agent")]
        public async Task<IActionResult> GetBackgroundImage()
        {
            try
            {
                var image = await _context.BackgroundImages
                    .OrderByDescending(b => b.UploadedAt)
                    .FirstOrDefaultAsync();

                if (image == null || image.ImageData == null)
                {
                    _logger.LogWarning("No background image found in the database.");
                    return NotFound("Background image not found.");
                }

                Response.Headers[HeaderNames.ContentDisposition] = "inline";
                return File(image.ImageData, image.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving background image.");
                return StatusCode(500, "An internal error occurred while retrieving the background image.");
            }
        }
    }
}
