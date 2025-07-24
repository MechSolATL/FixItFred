namespace MVP_Core.Pages.Admin
{
    public class SystemStatusModel : PageModel
    {
        public string MetaDescription { get; set; } = "Live status overview of Service-Atlanta's system components and services.";
        public string Keywords { get; set; } = "System Status, Uptime, Service Atlanta, Application Monitoring";
        public string Robots { get; set; } = "noindex, nofollow";
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public List<SystemComponentStatus> SystemComponents { get; set; } = [];

        public void OnGet()
        {
            // Replace this mock data with real-time monitoring data as needed
            SystemComponents =
            [
                new SystemComponentStatus { Name = "Customer Portal", IsOperational = true },
                new SystemComponentStatus { Name = "Email Verification", IsOperational = true },
                new SystemComponentStatus { Name = "Database Server", IsOperational = true },
                new SystemComponentStatus { Name = "Scheduler Engine", IsOperational = false },
                new SystemComponentStatus { Name = "Twilio Integration", IsOperational = true },
                new SystemComponentStatus { Name = "Admin Dashboard", IsOperational = true }
            ];
        }
    }

    public class SystemComponentStatus
    {
        public required string Name { get; set; }
        public bool IsOperational { get; set; }
    }
}
