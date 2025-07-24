// MVP_Core/Services/BackupReminderService.cs
namespace MVP_Core.Services
{
    public class BackupReminderService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly ILogger<BackupReminderService> _logger;
        private TimeZoneInfo _easternTimeZone;

        public BackupReminderService(ILogger<BackupReminderService> logger)
        {
            _logger = logger;
            try
            {
                _easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                _logger.LogWarning("Eastern Standard Timezone not found. Defaulting to UTC.");
                _easternTimeZone = TimeZoneInfo.Utc;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset nowEastern = TimeZoneInfo.ConvertTime(now, _easternTimeZone);

            DateTimeOffset nextRun = GetNextSaturdayAt1AM(nowEastern);
            TimeSpan initialDelay = nextRun - nowEastern;

            if (initialDelay < TimeSpan.Zero)
            {
                initialDelay = initialDelay.Add(TimeSpan.FromDays(7));
            }

            _timer = new Timer(DoBackupReminderSafe, null, initialDelay, TimeSpan.FromDays(7));

            _logger.LogInformation("?? BackupReminderService started. Next reminder in {Hours:F1} hours.", initialDelay.TotalHours);
            return Task.CompletedTask;
        }

        private static DateTimeOffset GetNextSaturdayAt1AM(DateTimeOffset now)
        {
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilSaturday == 0 && now.Hour >= 1)
            {
                daysUntilSaturday = 7; // If it's already Saturday after 1AM, schedule next week
            }

            DateTime nextSaturday = now.Date.AddDays(daysUntilSaturday).AddHours(1); // 1:00 AM
            return nextSaturday;
        }

        private void DoBackupReminderSafe(object? state)
        {
            try
            {
                _logger.LogWarning("?? [Backup Reminder] Please commit and push your GitHub repository for MechSolATL project!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inside BackupReminder Timer callback.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("?? BackupReminderService stopping.");
            _ = (_timer?.Change(Timeout.Infinite, 0));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
