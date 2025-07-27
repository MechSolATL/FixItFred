using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using MVP_Core.Services.Email;
using MVP_Core.Services.Admin;
using MVP_Core.Services.Dispatch;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MVP_Core.Pages.Customer
{
    public class RewardsModel : PageModel
    {
        private readonly CustomerPortalService _portalService;
        private readonly IServiceProvider _serviceProvider;
        public RewardsModel(CustomerPortalService portalService, IServiceProvider serviceProvider)
        {
            _portalService = portalService;
            _serviceProvider = serviceProvider;
        }
        public List<LoyaltyPointTransaction> Loyalty { get; set; } = new();
        public List<RewardTier> Tiers { get; set; } = new();
        public RewardTier? CurrentTier { get; set; }
        public async Task OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated || !User.HasClaim("IsCustomer", "true"))
            {
                return;
            }
            var email = User.Identity.Name ?? string.Empty;
            Loyalty = _portalService.GetLoyaltyTransactions(email);
            Tiers = _portalService.GetRewardTiers();
            var previousTier = HttpContext.Session.GetString("LastRewardTier");
            CurrentTier = Tiers.LastOrDefault(t => Loyalty.Sum(l => l.Points) >= t.PointsRequired);
            if (CurrentTier != null && (previousTier == null || previousTier != CurrentTier.Name))
            {
                // Generate PDF certificate
                var customer = _portalService.GetCustomer(email);
                var pdfPath = Path.Combine("wwwroot", "Certificates", $"Tier_{CurrentTier.Name}_{customer?.Id}.pdf");
                Directory.CreateDirectory(Path.GetDirectoryName(pdfPath)!);
                PDFPacketComposer.GenerateReadmePdf($"Congratulations {customer?.Name}! You have unlocked the {CurrentTier.Name} tier.\n{CurrentTier.Description}", pdfPath, DateTime.UtcNow, CurrentTier.Name);
                // Send email with PDF
                var emailService = _serviceProvider.GetService<EmailService>();
                if (emailService != null && customer?.Email != null)
                {
                    string dashboardUrl = Url.Page("/Customer/Dashboard");
                    await emailService.SendTierCertificateEmailAsync(
                        customer.Email,
                        customer.Name,
                        CurrentTier.Name,
                        CurrentTier.Description ?? string.Empty,
                        dashboardUrl,
                        pdfPath
                    );
                }
                HttpContext.Session.SetString("LastRewardTier", CurrentTier.Name);
            }
        }
    }
}
