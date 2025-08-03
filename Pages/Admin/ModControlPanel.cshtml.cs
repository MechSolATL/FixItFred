using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Pages.Admin
{
    public class ModControlPanelModel : PageModel
    {
        private readonly ILogger<ModControlPanelModel> _logger;

        public ModControlPanelModel(ILogger<ModControlPanelModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public ModLightDevice NewDevice { get; set; } = new();

        public List<ModLightDevice> ConnectedDevices { get; set; } = new();
        public List<ModAnalytics> ModStats { get; set; } = new();
        public string SystemStatus { get; set; } = "Online";
        public int TotalMods { get; set; }
        public int ActiveDevices { get; set; }
        public string LastPingTime { get; set; } = DateTime.Now.ToString("HH:mm:ss");

        public async Task OnGetAsync()
        {
            await LoadModData();
        }

        public async Task<IActionResult> OnPostRegisterDeviceAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadModData();
                return Page();
            }

            // Simulate device registration
            ConnectedDevices.Add(new ModLightDevice
            {
                Id = Guid.NewGuid().ToString(),
                Name = NewDevice.Name,
                Type = NewDevice.Type,
                Status = "Connected",
                LastPing = DateTime.Now,
                Location = NewDevice.Location
            });

            _logger.LogInformation("New MODLight device registered: {DeviceName} ({DeviceType})", 
                NewDevice.Name, NewDevice.Type);

            TempData["SuccessMessage"] = $"Device '{NewDevice.Name}' registered successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostTriggerPingAsync(string deviceId)
        {
            _logger.LogInformation("Ping triggered for device: {DeviceId}", deviceId);
            TempData["InfoMessage"] = "Ping signal sent to device";
            return RedirectToPage();
        }

        private async Task LoadModData()
        {
            // Simulate loading connected devices
            ConnectedDevices = new List<ModLightDevice>
            {
                new ModLightDevice
                {
                    Id = "mvp-core-001",
                    Name = "MVP-Core Main",
                    Type = "Core System",
                    Status = "Online",
                    LastPing = DateTime.Now.AddMinutes(-2),
                    Location = "Primary Server"
                },
                new ModLightDevice
                {
                    Id = "mvp-light-002",
                    Name = "CoreLight Mobile",
                    Type = "MVP-CoreLight",
                    Status = "Connected",
                    LastPing = DateTime.Now.AddMinutes(-1),
                    Location = "Field Device"
                },
                new ModLightDevice
                {
                    Id = "fx-broadcast-003",
                    Name = "FX Broadcaster",
                    Type = "Broadcast Layer",
                    Status = "Streaming",
                    LastPing = DateTime.Now.AddSeconds(-30),
                    Location = "Media Server"
                }
            };

            // Simulate MOD analytics
            ModStats = new List<ModAnalytics>
            {
                new ModAnalytics { Category = "MVP-Core", Count = 1, Usage = "High", Uptime = "99.9%" },
                new ModAnalytics { Category = "MVP-CoreLight", Count = 5, Usage = "Medium", Uptime = "98.5%" },
                new ModAnalytics { Category = "FX Broadcast", Count = 3, Usage = "Active", Uptime = "97.8%" },
                new ModAnalytics { Category = "Marketplace", Count = 2, Usage = "Low", Uptime = "100%" }
            };

            TotalMods = ConnectedDevices.Count;
            ActiveDevices = ConnectedDevices.Count(d => d.Status != "Offline");
            
            await Task.CompletedTask;
        }
    }

    public class ModLightDevice
    {
        public string Id { get; set; } = "";
        
        [Required]
        [Display(Name = "Device Name")]
        public string Name { get; set; } = "";
        
        [Required]
        [Display(Name = "Device Type")]
        public string Type { get; set; } = "";
        
        public string Status { get; set; } = "";
        public DateTime LastPing { get; set; }
        
        [Display(Name = "Location")]
        public string Location { get; set; } = "";
        
        public string GetStatusClass()
        {
            return Status switch
            {
                "Online" or "Connected" or "Streaming" => "status-heartbeat stable",
                "Warning" => "status-heartbeat warning",
                "Error" => "status-heartbeat critical",
                _ => "status-heartbeat offline"
            };
        }
    }

    public class ModAnalytics
    {
        public string Category { get; set; } = "";
        public int Count { get; set; }
        public string Usage { get; set; } = "";
        public string Uptime { get; set; } = "";
    }
}