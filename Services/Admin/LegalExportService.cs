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
using System.IO.Compression;
using System.Text.Json; // Use correct System.Text.Json namespace

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
            BillingInvoiceRecord? invoice = null;
            ServiceRequest? request = null;
            Customer? customer = null;
            List<TechnicianAuditLog> auditLogs = new();
            if (exportType == "ServiceRequest")
            {
                request = await _db.ServiceRequests.FindAsync(entityId);
                if (request != null)
                {
                    invoice = await _db.BillingInvoiceRecords.FirstOrDefaultAsync(i => i.CustomerEmail == request.Email);
                    customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == request.Email);
                    if (request.AssignedTechnicianId.HasValue)
                        auditLogs = await _db.TechnicianAuditLogs.Where(a => a.TechnicianId == request.AssignedTechnicianId.Value).ToListAsync();
                }
            }
            else if (exportType == "Customer")
            {
                customer = await _db.Customers.FindAsync(entityId);
                if (customer != null)
                {
                    invoice = await _db.BillingInvoiceRecords.FirstOrDefaultAsync(i => i.CustomerEmail == customer.Email);
                    auditLogs = await _db.TechnicianAuditLogs.Where(a => a.TechnicianId == customer.Id).ToListAsync();
                }
            }
            var pdfPaths = new List<string>();
            var tempDir = Path.Combine(Path.GetTempPath(), $"LegalExport_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);
            if (invoice != null)
            {
                var invoicePdf = Path.Combine(tempDir, "Invoice.pdf");
                await SprintPdfArchive.GenerateBrandedEstimatePdfAsync(invoice, invoicePdf, "/assets/company-logo.png", "NovaOps | 555-1234 | support@novaops.com", "Legal disclaimers", "Legal terms", invoice.Status ?? "", new[] { "Labor", "Parts", "Tax" });
                pdfPaths.Add(invoicePdf);
            }
            // Add legal templates as static HTML
            var privacyPath = Path.Combine(tempDir, "PrivacyPolicy.html");
            var termsPath = Path.Combine(tempDir, "TermsAndConditions.html");
            if (File.Exists("Pages/Legal/PrivacyPolicy.cshtml"))
                File.Copy("Pages/Legal/PrivacyPolicy.cshtml", privacyPath, true);
            if (File.Exists("Pages/Legal/TermsAndConditions.cshtml"))
                File.Copy("Pages/Legal/TermsAndConditions.cshtml", termsPath, true);
            // Serialize audit logs
            var auditLogPath = Path.Combine(tempDir, "AuditLog.json");
            File.WriteAllText(auditLogPath, JsonSerializer.Serialize(auditLogs)); // Use System.Text.Json.JsonSerializer
            // Bundle as ZIP
            var zipPath = Path.Combine(tempDir, "LegalExportPacket.zip");
            ZipFile.CreateFromDirectory(tempDir, zipPath);
            return zipPath;
        }
    }
}
