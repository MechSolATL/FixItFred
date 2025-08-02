using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Admin;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Admin.AdminUsers
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public string JobFunction { get; set; } = string.Empty;
        [BindProperty]
        public int SkillLevel { get; set; }
        [BindProperty]
        public List<string> EnabledModules { get; set; } = new();
        [BindProperty]
        public int WizardStep { get; set; } = 1;
        [BindProperty]
        public int Id { get; set; }
        public AdminUser? AdminUser { get; set; }

        public void OnGet(int id)
        {
            // Load user and prefill wizard
            // (Assume injected db context, omitted for brevity)
            // AdminUser = db.AdminUsers.Find(id);
            // if (AdminUser != null) { ... }
        }

        public IActionResult OnPost(string action)
        {
            if (action == "Next")
            {
                if (WizardStep < 4) WizardStep++;
            }
            else if (action == "Back")
            {
                if (WizardStep > 1) WizardStep--;
            }
            else if (action == "Save")
            {
                // Save logic here (update AdminUser)
                // db.SaveChanges();
                return RedirectToPage("Index");
            }
            // Rehydrate state for next step
            return Page();
        }
    }
}
