using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace MVP_Core.Pages.QA
{
    // Sprint 32.3 - QA Milestone Setup
    public class ReleaseChecklistModel : PageModel
    {
        public List<string> Checklist { get; set; } = new()
        {
            "ETA override flow",
            "Technician update",
            "Admin metrics accurate",
            "GPS + ETA broadcast",
            "Schedule accept/reject",
            "Audit log creation"
        };
        [BindProperty]
        public List<bool> Checked { get; set; } = new();
        public void OnGet()
        {
            if (TempData["Checked"] is List<bool> checkedList)
                Checked = checkedList;
            else
                Checked = new List<bool>(new bool[Checklist.Count]);
        }
        public IActionResult OnPost(List<string> checkedItems)
        {
            Checked = new List<bool>(new bool[Checklist.Count]);
            if (checkedItems != null)
            {
                foreach (var idxStr in checkedItems)
                {
                    if (int.TryParse(idxStr, out int idx) && idx >= 0 && idx < Checked.Count)
                        Checked[idx] = true;
                }
            }
            TempData["Checked"] = Checked;
            return RedirectToPage();
        }
    }
}
