using System;
using System.IO;
using System.Threading.Tasks;
using MVP_Core.Data.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;

namespace MVP_Core.Services.Admin
{
    public class LegalExportService
    {
        private readonly ApplicationDbContext _db;
        public LegalExportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> ExportLegalPacketAsync(int entityId, string exportType)
        {
            // Gather required documents
            BillingInvoiceRecord? invoice = null;
            ServiceRequest? request = null;
            Customer? customer = null;
            List<TechnicianAuditLog> auditLogs = new();
            if (exportType == "ServiceRequest")
            {
                request = await _db.ServiceRequests.FindAsync(entityId);
                if (request != null)
                {
                    invoice = await _db.BillingInvoiceRecords.FirstOrDefaultAsync(i => i.Id == request.InvoiceId);
                    customer = await _db.Customers.FindAsync(request.CustomerId);
                    auditLogs = await _db.TechnicianAuditLogs.Where(a => a.TechnicianId == request.TechnicianId).ToListAsync();
                }
            }
            else if (exportType == "Customer")
            {
                customer = await _db.Customers.FindAsync(entityId);
                invoice = await _db.BillingInvoiceRecords.FirstOrDefaultAsync(i => i.CustomerEmail == customer.Email);
                auditLogs = await _db.TechnicianAuditLogs.Where(a => a.TechnicianId == customer.Id).ToListAsync();
            }
            // Generate PDFs
            var pdfPaths = new List<string>();
            var tempDir = Path.Combine(Path.GetTempPath(), $"LegalExport_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);
            if (invoice != null)
            {
                var invoicePdf = Path.Combine(tempDir, "Invoice.pdf");
                await SprintPdfArchive.GenerateBrandedEstimatePdfAsync(invoice, invoicePdf, "/assets/company-logo.png", "NovaOps | 555-1234 | support@novaops.com", "Legal disclaimers", "Legal terms", invoice.Status, new[] { "Labor", "Parts", "Tax" });
                pdfPaths.Add(invoicePdf);
            }
            // Add legal templates as static HTML
            var privacyHtml = File.ReadAllText("Pages/Legal/PrivacyPolicy.cshtml");
            var termsHtml = File.ReadAllText("Pages/Legal/TermsAndConditions.cshtml");
            File.WriteAllText(Path.Combine(tempDir, "PrivacyPolicy.html"), privacyHtml);
            File.WriteAllText(Path.Combine(tempDir, "TermsAndConditions.html"), termsHtml);
            // Serialize audit logs
            var auditLogPath = Path.Combine(tempDir, "AuditLog.json");
            File.WriteAllText(auditLogPath, System.Text.Json.JsonSerializer.Serialize(auditLogs));
            // Bundle as ZIP
            var zipPath = Path.Combine(tempDir, "LegalExportPacket.zip");
            using (var archive = new System.IO.Compression.ZipArchive(new FileStream(zipPath, FileMode.Create), System.IO.Compression.ZipArchiveMode.Create))
            {
                foreach (var file in Directory.GetFiles(tempDir))
                {
                    if (!file.EndsWith(".zip"))
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
            return zipPath;
        }
    }
}
