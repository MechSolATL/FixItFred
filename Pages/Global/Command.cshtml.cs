using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.ViewModels;
using MVP_Core.Services.Metrics;
using MVP_Core.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Global
{
    public class CommandModel : PageModel
    {
        private readonly MetricsEngineService _metricsEngine;
        private readonly ForecastingEngine _forecastingEngine;
        public CommandModel(MetricsEngineService metricsEngine, ForecastingEngine forecastingEngine)
        {
            _metricsEngine = metricsEngine;
            _forecastingEngine = forecastingEngine;
        }
        public List<MetricsCardViewModel> Metrics { get; set; } = new();
        public ForecastMetricsViewModel? Forecast { get; set; }
        public async Task OnGetAsync()
        {
            Metrics = await _metricsEngine.GetGlobalMetricsAsync();
            Forecast = await _forecastingEngine.GetForecastAsync();
        }
    }
}
