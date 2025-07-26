using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class TechnicianPayModel : PageModel
    {
        private readonly TechnicianPayService _payService;
        private readonly TechnicianBonusService _bonusService;
        public int TechId { get; set; }
        public List<TechnicianPayRecord> PayHistory { get; set; } = new();
        public List<TechnicianBonusLog> BonusHistory { get; set; } = new();
        public TechnicianPayModel(TechnicianPayService payService, TechnicianBonusService bonusService)
        {
            _payService = payService;
            _bonusService = bonusService;
        }
        public void OnGet(int techId, TechnicianBonusType? bonusType = null, DateTime? awardDate = null)
        {
            TechId = techId;
            if (techId > 0)
            {
                PayHistory = _payService.GetPayHistory(techId);
                if (bonusType.HasValue)
                    BonusHistory = _bonusService.GetBonusesByType(bonusType.Value).Where(b => b.TechnicianId == techId).ToList();
                else if (awardDate.HasValue)
                    BonusHistory = _bonusService.GetBonusesByDate(awardDate.Value).Where(b => b.TechnicianId == techId).ToList();
                else
                    BonusHistory = _bonusService.GetBonuses(techId);
            }
        }
    }
}
