using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Data;
using Services;
using Data.Models;

namespace Pages.Admin
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
