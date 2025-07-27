using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MVP_Core.Data;
using MVP_Core.Services.HumorOps;
using MVP_Core.Services.Admin;

namespace MVP_Core.Services.Background
{
    public class RoastRouletteHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RoastRouletteHostedService> _logger;
        private const int DeliveryCap = 10; // Max roasts per run
        private static readonly TimeSpan RunInterval = TimeSpan.FromHours(72);
        private static readonly int RunHourET = 15; // 3 PM ET

        public RoastRouletteHostedService(IServiceProvider serviceProvider, ILogger<RoastRouletteHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddHours(RunHourET);
                if (now > nextRun) nextRun = nextRun.AddDays(3);
                var delay = nextRun - now;
                _logger.LogInformation($"RoastRouletteHostedService sleeping for {delay.TotalMinutes:F0} minutes until next run at {nextRun:u}.");
                await Task.Delay(delay, stoppingToken);
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roastEngineService = scope.ServiceProvider.GetRequiredService<RoastEngineService>();
                var rouletteEngine = scope.ServiceProvider.GetRequiredService<RoastRouletteEngine>();
                try
                {
                    var eligibleTargets = await rouletteEngine.GetWeightedEligibleTargetsAsync();
                    int delivered = 0;
                    foreach (var target in eligibleTargets)
                    {
                        if (delivered >= DeliveryCap) break;
                        var tier = target.RoastTierPreference != null && Enum.TryParse<RoastTier>(target.RoastTierPreference, out var prefTier)
                            ? prefTier : rouletteEngine.GetNextTier();
                        var template = await rouletteEngine.GetRandomRoastTemplateAsync(tier);
                        if (template == null) continue;
                        await rouletteEngine.DropRoastAsync(target, template);
                        delivered++;
                    }
                    _logger.LogInformation($"RoastRouletteHostedService: {delivered} roasts delivered.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RoastRouletteHostedService: Error during roast roulette drop.");
                }
            }
        }
    }
}
