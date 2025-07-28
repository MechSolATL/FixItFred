using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.ViewModels;
using MVP_Core.Services.Metrics;
using MVP_Core.Models.ViewModels;
using MVP_Core.Services.Actions;
using MVP_Core.Services.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Global
{
    public class CommandModel : PageModel
    {
        private readonly MetricsEngineService _metricsEngine;
        private readonly ForecastingEngine _forecastingEngine;
        private readonly ProActionQueueEngine _proActionQueueEngine;
        private readonly ProcessMonitorService _processMonitorService;
        public CommandModel(MetricsEngineService metricsEngine, ForecastingEngine forecastingEngine, ProActionQueueEngine proActionQueueEngine, ProcessMonitorService processMonitorService)
        {
            _metricsEngine = metricsEngine;
            _forecastingEngine = forecastingEngine;
            _proActionQueueEngine = proActionQueueEngine;
            _processMonitorService = processMonitorService;
        }
        public List<MetricsCardViewModel> Metrics { get; set; } = new();
        public ForecastMetricsViewModel? Forecast { get; set; }
        public List<ProActionCardViewModel> ProActions { get; set; } = new();
        public List<ProcessStatusViewModel> ProcessStatuses { get; set; } = new();
        public async Task OnGetAsync()
        {
            Metrics = await _metricsEngine.GetGlobalMetricsAsync();
            Forecast = await _forecastingEngine.GetForecastAsync();
            ProActions = _proActionQueueEngine.GetProActions();
            ProcessStatuses = await _processMonitorService.GetAllProcessStatusesAsync();
        }
    }
}
