using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using MVP_Core.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MVP_Core.Pages.Admin
{
    public class PromotionsModel : PageModel
    {
        private readonly PromotionEngineService _promoService;
        private readonly ApplicationDbContext _db;
        public PromotionsModel(PromotionEngineService promoService, ApplicationDbContext db)
        {
            _promoService = promoService;
            _db = db;
        }
        public List<PromotionEvent> Promotions { get; set; } = new();
        public void OnGet()
        {
            Promotions = _promoService.GetActivePromotions();
        }
        public IActionResult OnGetToggle(int id)
        {
            var promo = _db.PromotionEvents.FirstOrDefault(p => p.Id == id);
            if (promo != null)
            {
                promo.Active = !promo.Active;
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
        // Add OnPostCreate, OnPostEdit, etc. for full CRUD as needed
    }
}
