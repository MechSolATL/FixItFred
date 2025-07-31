using MVP_Core.Services.Admin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Data;

namespace Services.Background
{
    public class RoastEvolutionHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public RoastEvolutionHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ => await RunEvolutionAsync(), null, TimeSpan.Zero, TimeSpan.FromDays(7));
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }
        private async Task RunEvolutionAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var feedbackService = new RoastFeedbackService(db);
            var engine = new RoastEvolutionEngine(db, feedbackService);
            await engine.AnalyzeAndEvolveVaultAsync();
            await engine.TagLegacyAndGenerateAIAsync();
        }
    }
}
