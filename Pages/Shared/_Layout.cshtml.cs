using Microsoft.AspNetCore.Mvc.RazorPages;
using Wangkanai.Detection;

namespace MVP_Core.Pages.Shared
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
            var deviceType = _device.Device.Type.ToString();
            ViewData["DeviceType"] = deviceType;
        }
    }
}
