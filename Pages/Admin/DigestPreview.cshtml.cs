// Sprint 44 – Message Export + Digest
using MVP_Core.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;

namespace Pages.Admin
{
    public class DigestPreviewModel : PageModel
    {
        private readonly NotificationDigestService _digestService;
        public DigestPreviewModel(NotificationDigestService digestService)
        {
            _digestService = digestService;
        }
        public NotificationDigestService.DigestSummary? Digest { get; set; }
        public async Task OnGetAsync()
        {
            Digest = await _digestService.GenerateDigestPreviewAsync();
        }
    }
}
