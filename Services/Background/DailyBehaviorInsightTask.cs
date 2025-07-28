using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVP_Core.Services.Admin;
using MVP_Core.Data;

namespace MVP_Core.Services.Background
{
    public class DailyBehaviorInsightTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public DailyBehaviorInsightTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddHours(0); // midnight
                var delay = nextRun - now;
                if (delay.TotalMilliseconds > 0)
                    await Task.Delay(delay, stoppingToken);
                await RunBatchAsync();
            }
        }

        private async Task RunBatchAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var engine = scope.ServiceProvider.GetRequiredService<BehaviorPatternEngine>();
            var trust = scope.ServiceProvider.GetRequiredService<TrustIndexScoringService>();
            var techIds = db.Technicians.Select(t => t.Id).ToList();
            foreach (var techId in techIds)
            {
                var patterns = engine.AnalyzeTechnicianBehavior(techId);
                var trustIndex = trust.ComputeTrustIndex(patterns);
                trust.CacheTrustIndex(techId, trustIndex);
            }
            await Task.CompletedTask;
        }
    }
}
