using Helpers;
using Services;

namespace Pages.Tools
{
    [ValidateAntiForgeryToken]
    public class HashPasswordModel : PageModel
    {
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;

        public HashPasswordModel(ISeoService seoService, IDeviceResolver deviceResolver)
        {
            _seoService = seoService;
            _deviceResolver = deviceResolver;
        }

        [BindProperty]
        public string InputPassword { get; set; } = string.Empty;

        public string? GeneratedHash { get; set; }

        public async Task OnGetAsync()
        {
            var seo = await _seoService.GetSeoByPageNameAsync("Tools/HashPassword");
            ViewData["Title"] = seo?.Title ?? "Hash Password";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);
        }

        public void OnPost()
        {
            if (!string.IsNullOrWhiteSpace(InputPassword))
            {
                GeneratedHash = PasswordHasher.HashPassword(InputPassword);
            }
        }
    }
}
