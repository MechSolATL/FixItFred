using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services.CommissionCalculator;

namespace MVP_Core.Pages.Tech
{
    public class CommissionViewModel : PageModel
    {
        private readonly ICommissionCalculatorService _calculator;

        public CommissionViewModel(ICommissionCalculatorService calculator)
        {
            _calculator = calculator;
        }

        [BindProperty]
        public decimal SaleAmount { get; set; }

        [BindProperty]
        public string TechnicianRank { get; set; } = "Bronze";

        public decimal Commission { get; private set; }

        public void OnPost()
        {
            Commission = _calculator.CalculateCommission(SaleAmount, TechnicianRank);
        }
    }
}