using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianPayModel : PageModel
    {
        private readonly TechnicianPayService _payService;
        public int TechId { get; set; }
        public List<TechnicianPayRecord> PayHistory { get; set; } = new();
        public TechnicianPayModel(TechnicianPayService payService)
        {
            _payService = payService;
        }
        public void OnGet(int techId)
        {
            TechId = techId;
            if (techId > 0)
                PayHistory = _payService.GetPayHistory(techId);
        }
    }
}
