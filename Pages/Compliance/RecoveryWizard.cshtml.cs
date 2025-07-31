using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using ViewModels;

namespace Pages.Compliance
{
    public class RecoveryWizardModel : PageModel
    {
        [BindProperty]
        public ComplianceRecoveryViewModel ViewModel { get; set; } = new();

        public void OnGet()
        {
            // Example: Populate with expired docs (replace with real service call)
            ViewModel.ExpiredDocuments = new List<ExpiredDocument>
            {
                new ExpiredDocument { DocumentType = "Insurance", LastValidDate = null, FileStatus = "Expired", RequiresCertificateHolder = true },
                new ExpiredDocument { DocumentType = "License", LastValidDate = null, FileStatus = "Expired", RequiresCertificateHolder = false },
                new ExpiredDocument { DocumentType = "Certification", LastValidDate = null, FileStatus = "Expired", RequiresCertificateHolder = false }
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate uploads
            foreach (var doc in ViewModel.ExpiredDocuments)
            {
                var file = Request.Form.Files[$"Uploads[{doc.DocumentType}]"];
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError($"Uploads[{doc.DocumentType}]", $"{doc.DocumentType} is required.");
                    continue;
                }
                if (file.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError($"Uploads[{doc.DocumentType}]", "File size must be under 5MB.");
                }
                var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(file.ContentType))
                {
                    ModelState.AddModelError($"Uploads[{doc.DocumentType}]", "Invalid file type.");
                }
                // For insurance, require certificate holder in file name or metadata (simple check)
                if (doc.RequiresCertificateHolder && !file.FileName.ToLower().Contains("certificate"))
                {
                    ModelState.AddModelError($"Uploads[{doc.DocumentType}]", "Certificate Holder required in file name.");
                }
            }
            if (!ModelState.IsValid)
            {
                OnGet(); // Repopulate docs
                return Page();
            }
            // TODO: Save uploads, log, and trigger review
            TempData["Success"] = "Documents submitted for review.";
            return RedirectToPage("/Compliance/RecoveryWizard");
        }
    }
}
