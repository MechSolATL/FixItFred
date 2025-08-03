using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Pages.Admin
{
    public class WatchtowerDashboardModel : PageModel
    {
        private readonly ILogger<WatchtowerDashboardModel> _logger;

        public WatchtowerDashboardModel(ILogger<WatchtowerDashboardModel> logger)
        {
            _logger = logger;
        }

        public List<PlatformMetric> AllPlatforms { get; set; } = new();
        public List<ModuleStatus> AllModules { get; set; } = new();
        public List<OrganizationHealth> AllOrganizations { get; set; } = new();
        public List<LiveFeedItem> LiveFeed { get; set; } = new();
        public WatchtowerStats GlobalStats { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadWatchtowerData();
            _logger.LogInformation("Watchtower Dashboard accessed - Nova's Command Center online");
        }

        private async Task LoadWatchtowerData()
        {
            // Simulate global platform metrics
            AllPlatforms = new List<PlatformMetric>
            {
                new PlatformMetric
                {
                    Name = "MVP-Core",
                    Status = "Operational",
                    Uptime = "99.9%",
                    ActiveUsers = 1247,
                    TotalTransactions = 45892,
                    LastUpdate = DateTime.Now.AddMinutes(-2)
                },
                new PlatformMetric
                {
                    Name = "MVP-CoreLight",
                    Status = "Stable",
                    Uptime = "98.5%",
                    ActiveUsers = 523,
                    TotalTransactions = 12043,
                    LastUpdate = DateTime.Now.AddMinutes(-1)
                },
                new PlatformMetric
                {
                    Name = "FX Broadcast Layer",
                    Status = "Streaming",
                    Uptime = "97.8%",
                    ActiveUsers = 89,
                    TotalTransactions = 3421,
                    LastUpdate = DateTime.Now.AddSeconds(-30)
                }
            };

            // Simulate module status across all systems
            AllModules = new List<ModuleStatus>
            {
                new ModuleStatus { Name = "Quote Engine", Status = "Active", Usage = 234, Performance = "High" },
                new ModuleStatus { Name = "FX Visual Flow", Status = "Active", Usage = 156, Performance = "Optimal" },
                new ModuleStatus { Name = "Marketplace", Status = "Running", Usage = 89, Performance = "Good" },
                new ModuleStatus { Name = "Loyalty System", Status = "Active", Usage = 445, Performance = "High" },
                new ModuleStatus { Name = "Analytics Engine", Status = "Processing", Usage = 678, Performance = "Peak" },
                new ModuleStatus { Name = "Notification Hub", Status = "Active", Usage = 1023, Performance = "High" }
            };

            // Simulate organization health data
            AllOrganizations = new List<OrganizationHealth>
            {
                new OrganizationHealth { Name = "MechSolATL", Health = "Excellent", Score = 98, ActiveProjects = 12 },
                new OrganizationHealth { Name = "Field Operations", Health = "Good", Score = 87, ActiveProjects = 8 },
                new OrganizationHealth { Name = "Support Division", Health = "Stable", Score = 92, ActiveProjects = 5 },
                new OrganizationHealth { Name = "Development Team", Health = "Peak", Score = 99, ActiveProjects = 15 }
            };

            // Simulate live activity feed
            LiveFeed = new List<LiveFeedItem>
            {
                new LiveFeedItem { Time = DateTime.Now.AddSeconds(-10), Event = "FX Prize Wheel activated", Type = "Marketplace", Severity = "Info" },
                new LiveFeedItem { Time = DateTime.Now.AddMinutes(-2), Event = "New technician onboarded", Type = "Onboarding", Severity = "Success" },
                new LiveFeedItem { Time = DateTime.Now.AddMinutes(-5), Event = "Quote Engine served 1000th quote", Type = "System", Severity = "Achievement" },
                new LiveFeedItem { Time = DateTime.Now.AddMinutes(-8), Event = "MVP-CoreLight device connected", Type = "Device", Severity = "Info" },
                new LiveFeedItem { Time = DateTime.Now.AddMinutes(-12), Event = "Watchtower scan completed", Type = "System", Severity = "Success" }
            };

            // Calculate global statistics
            GlobalStats = new WatchtowerStats
            {
                TotalUsers = AllPlatforms.Sum(p => p.ActiveUsers),
                TotalTransactions = AllPlatforms.Sum(p => p.TotalTransactions),
                AverageUptime = AllPlatforms.Average(p => double.Parse(p.Uptime.TrimEnd('%'))),
                ActiveModules = AllModules.Count(m => m.Status == "Active"),
                TotalOrganizations = AllOrganizations.Count,
                SystemHealth = "Operational"
            };

            await Task.CompletedTask;
        }
    }

    public class PlatformMetric
    {
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        public string Uptime { get; set; } = "";
        public int ActiveUsers { get; set; }
        public int TotalTransactions { get; set; }
        public DateTime LastUpdate { get; set; }

        public string GetStatusClass()
        {
            return Status switch
            {
                "Operational" or "Active" or "Stable" => "status-heartbeat stable",
                "Streaming" or "Running" => "status-heartbeat stable",
                "Warning" => "status-heartbeat warning",
                "Error" or "Down" => "status-heartbeat critical",
                _ => "status-heartbeat offline"
            };
        }
    }

    public class ModuleStatus
    {
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        public int Usage { get; set; }
        public string Performance { get; set; } = "";

        public string GetPerformanceClass()
        {
            return Performance switch
            {
                "Peak" or "Optimal" => "text-success",
                "High" or "Good" => "text-primary",
                "Medium" => "text-warning",
                _ => "text-secondary"
            };
        }
    }

    public class OrganizationHealth
    {
        public string Name { get; set; } = "";
        public string Health { get; set; } = "";
        public int Score { get; set; }
        public int ActiveProjects { get; set; }

        public string GetHealthClass()
        {
            return Score switch
            {
                >= 95 => "text-success",
                >= 85 => "text-primary",
                >= 70 => "text-warning",
                _ => "text-danger"
            };
        }
    }

    public class LiveFeedItem
    {
        public DateTime Time { get; set; }
        public string Event { get; set; } = "";
        public string Type { get; set; } = "";
        public string Severity { get; set; } = "";

        public string GetSeverityClass()
        {
            return Severity switch
            {
                "Success" or "Achievement" => "text-success",
                "Info" => "text-info",
                "Warning" => "text-warning",
                "Error" => "text-danger",
                _ => "text-muted"
            };
        }

        public string GetIcon()
        {
            return Type switch
            {
                "Marketplace" => "🛍️",
                "Onboarding" => "👋",
                "System" => "⚙️",
                "Device" => "📱",
                _ => "ℹ️"
            };
        }
    }

    public class WatchtowerStats
    {
        public int TotalUsers { get; set; }
        public int TotalTransactions { get; set; }
        public double AverageUptime { get; set; }
        public int ActiveModules { get; set; }
        public int TotalOrganizations { get; set; }
        public string SystemHealth { get; set; } = "";
    }
}