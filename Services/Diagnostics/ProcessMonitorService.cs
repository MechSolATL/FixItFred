using System;
using System.Collections.Generic;
using MVP_Core.ViewModels;

namespace MVP_Core.Services.Diagnostics
{
    public class ProcessMonitorService
    {
        public List<ProcessStatusViewModel> GetAllProcessStatuses()
        {
            // In a real implementation, these would be live checks
            return new List<ProcessStatusViewModel>
            {
                new ProcessStatusViewModel {
                    Name = "Dispatch Engine",
                    Status = ProcessHealthStatus.Healthy,
                    LastChecked = DateTime.UtcNow,
                    SuggestedAction = "No action needed"
                },
                new ProcessStatusViewModel {
                    Name = "Email Delivery",
                    Status = ProcessHealthStatus.Warning,
                    LastChecked = DateTime.UtcNow,
                    SuggestedAction = "Verify SMTP credentials / spam flags"
                },
                new ProcessStatusViewModel {
                    Name = "SMS Gateway",
                    Status = ProcessHealthStatus.Critical,
                    LastChecked = DateTime.UtcNow,
                    SuggestedAction = "Check Twilio API keys and balance"
                },
                new ProcessStatusViewModel {
                    Name = "Alerts System",
                    Status = ProcessHealthStatus.Healthy,
                    LastChecked = DateTime.UtcNow,
                    SuggestedAction = "No action needed"
                },
                new ProcessStatusViewModel {
                    Name = "Billing Processor",
                    Status = ProcessHealthStatus.Unknown,
                    LastChecked = DateTime.UtcNow,
                    SuggestedAction = "Check integration logs"
                }
            };
        }
    }
}
