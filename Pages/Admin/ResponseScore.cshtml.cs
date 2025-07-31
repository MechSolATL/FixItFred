using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Data.ViewModels;
using Services.Admin;

namespace Pages.Admin
{
    public class ResponseScoreModel : PageModel
    {
        private readonly TechnicianResponseService _service;
        public List<ResponseScorecard> Scorecards { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public DateTime? FilterStart { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? FilterEnd { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FilterSource { get; set; }

        public ResponseScoreModel(TechnicianResponseService service)
        {
            _service = service;
        }

        public async Task OnGetAsync()
        {
            Scorecards = await _service.GenerateResponseScorecardsAsync(FilterStart, FilterEnd, FilterSource);
            if (Request.Query["export"] == "csv")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Rank,Technician,AvgResponseSeconds,TotalResponses,LateCount");
                foreach (var card in Scorecards)
                {
                    csv.AppendLine($"{card.Rank},{card.TechnicianName},{card.AverageResponseSeconds},{card.TotalResponses},{card.LateCount}");
                }
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                Response.Headers["Content-Disposition"] = "attachment; filename=response-scorecards.csv";
                Response.ContentType = "text/csv";
                await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                Response.Body.Close();
            }
        }
    }
}
