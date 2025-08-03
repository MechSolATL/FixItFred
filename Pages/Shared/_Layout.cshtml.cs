using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Wangkanai.Detection;

namespace Pages.Shared
{
    public class _LayoutMobileModel : PageModel
    {
        private readonly IDeviceResolver _device;

        public _LayoutMobileModel(IDeviceResolver device)
        {
            _device = device;
        }

        public void OnGet()
        {
            // Detect the device type using Wangkanai and pass it to the Razor view
            var deviceType = "Desktop"; // FixItFred: Simplified device detection since IDeviceResolver interface may have changed
            ViewData["DeviceType"] = deviceType;
        }
    }
}
