// Sprint 54.1: Estimate PDF generation, signature save, decision status, notification trigger
using Data;
using Data.Models;
using Services.Admin;
using Services.Dispatch;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class EstimateService
    {
        private readonly ApplicationDbContext _db;
        private readonly NotificationDispatchEngine _dispatchEngine;
        public EstimateService(ApplicationDbContext db, NotificationDispatchEngine dispatchEngine)
        {
            _db = db;
            _dispatchEngine = dispatchEngine;
        }
        public async Task<string> GenerateBrandedEstimatePdfAsync(BillingInvoiceRecord invoice, string logoPath, string[] lineItems)
        {
            string outputPath = Path.Combine("Docs", "Estimates", $"Estimate_{invoice.Id}.pdf");
            string contactInfo = "NovaOps | 555-1234 | support@novaops.com";
            string disclaimers = "All estimates are subject to change based on site conditions. Contact us for details.";
            string terms = "Payment due upon completion. Warranty terms apply as per contract.";
            await SprintPdfArchive.GenerateBrandedEstimatePdfAsync(invoice, outputPath, logoPath, contactInfo, disclaimers, terms, invoice.Status, lineItems, invoice.CustomerSignaturePath);
            return outputPath;
        }
        public void SaveSignature(Guid invoiceId, string signaturePath, string acknowledgedBy)
        {
            var invoice = _db.BillingInvoiceRecords.Find(invoiceId);
            if (invoice == null) return;
            invoice.CustomerSignaturePath = signaturePath;
            invoice.IsAcknowledged = true;
            invoice.AcknowledgedBy = acknowledgedBy;
            invoice.AcknowledgedDate = DateTime.UtcNow;
            _db.SaveChanges();
            _dispatchEngine.TriggerEstimateAcknowledged(invoice.CustomerEmail, invoiceId);
        }
        public void SaveDecision(Guid invoiceId, bool accepted)
        {
            var invoice = _db.BillingInvoiceRecords.Find(invoiceId);
            if (invoice == null) return;
            invoice.WasAccepted = accepted;
            invoice.DecisionDate = DateTime.UtcNow;
            _db.SaveChanges();
            _dispatchEngine.TriggerEstimateDecision(invoice.CustomerEmail, invoiceId, accepted);
        }
    }
}
