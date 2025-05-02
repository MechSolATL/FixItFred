using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MVP_Core.Helpers
{
    public class BackupReminderService : BackgroundService
    {
        private readonly ILogger<BackupReminderService> _logger;
        private readonly TimeZoneInfo _easternTimeZone;
        private static readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);

        public BackupReminderService(ILogger<BackupReminderService> logger)
        {
            _logger = logger;
            _easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // Eventually move to config if cross-platform
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔄 BackupReminderService started.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _easternTimeZone);

                    if (now.DayOfWeek == DayOfWeek.Saturday && now.Hour == 1)
                    {
                        _logger.LogInformation("🛡️ [Backup Reminder] Push code to GitHub! (Automatic Alert @ {Time})", now);

                        try
                        {
                            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Prevent spam after firing
                        }
                        catch (TaskCanceledException)
                        {
                            // Safe exit
                        }
                    }

                    try
                    {
                        await Task.Delay(_checkInterval, stoppingToken); // Normal cycle
                    }
                    catch (TaskCanceledException)
                    {
                        // Safe exit
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "❌ BackupReminderService encountered an error and stopped.");
                throw; // Re-throw critical background service failures
            }

            _logger.LogInformation("🛑 BackupReminderService stopping gracefully.");
        }
    }
}
