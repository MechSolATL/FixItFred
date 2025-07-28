using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.ViewModels;
using MVP_Core.Services.Metrics;
using MVP_Core.Models.ViewModels;
using MVP_Core.Services.Actions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Global
{
    public class CommandModel : PageModel
    {
        private readonly MetricsEngineService _metricsEngine;
        private readonly ForecastingEngine _forecastingEngine;
        private readonly ProActionQueueEngine _proActionQueueEngine;
        public CommandModel(MetricsEngineService metricsEngine, ForecastingEngine forecastingEngine, ProActionQueueEngine proActionQueueEngine)
        {
            _metricsEngine = metricsEngine;
            _forecastingEngine = forecastingEngine;
            _proActionQueueEngine = proActionQueueEngine;
        }
        public List<MetricsCardViewModel> Metrics { get; set; } = new();
        public ForecastMetricsViewModel? Forecast { get; set; }
        public List<ProActionCardViewModel> ProActions { get; set; } = new();
        public async Task OnGetAsync()
        {
            Metrics = await _metricsEngine.GetGlobalMetricsAsync();
            Forecast = await _forecastingEngine.GetForecastAsync();
            ProActions = _proActionQueueEngine.GetProActions();
        }
    }
}
