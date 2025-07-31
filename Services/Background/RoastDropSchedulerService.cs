using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Data;
using Services.Admin;

namespace Services.Background
{
    public class RoastDropSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RoastDropSchedulerService> _logger;
        private readonly IConfiguration _configuration;

        public RoastDropSchedulerService(IServiceProvider serviceProvider, ILogger<RoastDropSchedulerService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool enabled = _configuration.GetValue("RoastScheduler:Enabled", true);
            if (!enabled)
            {
                _logger.LogInformation("RoastDropSchedulerService is disabled via configuration.");
                return;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                // Configurable hour (default 13 = 9AM ET)
                int hour = _configuration.GetValue("RoastScheduler:Hour", 13);
                var nextRun = now.Date.AddDays((int)DayOfWeek.Monday - (int)now.DayOfWeek).AddHours(hour);
                if (now > nextRun) nextRun = nextRun.AddDays(7);
                var delay = nextRun - now;
                _logger.LogInformation($"RoastDropSchedulerService sleeping for {delay.TotalMinutes:F0} minutes until next run at {nextRun:u}.");
                await Task.Delay(delay, stoppingToken);
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var service = scope.ServiceProvider.GetRequiredService<RoastEngineService>();
                try
                {
                    await service.DropWeeklyRoastsAsync();
                    _logger.LogInformation("RoastDropSchedulerService: Weekly roasts dropped successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RoastDropSchedulerService: Error during roast drop.");
                }
            }
        }
    }
}
