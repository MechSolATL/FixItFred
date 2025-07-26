using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    /// <summary>
    /// Sprint 60.0: Regenerates and archives PDF receipts, verifies signed docs, and manages archive storage.
    /// </summary>
    public class ReceiptArchiveService
    {
        private readonly ApplicationDbContext _db;
        public ReceiptArchiveService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<string> RegenerateAndArchiveReceiptAsync(int serviceRequestId)
        {
            var request = _db.ServiceRequests.Find(serviceRequestId);
            if (request == null) return string.Empty;
            var completedDate = request.CompletedDate.HasValue ? request.CompletedDate.Value.Date : (DateTime?)null;
            var invoice = _db.BillingInvoiceRecords.FirstOrDefault(i => i.CustomerEmail == request.Email && completedDate.HasValue && i.InvoiceDate.Date == completedDate.Value);
            if (invoice == null) return string.Empty;
            // Generate PDF (simulate or use SprintPdfArchive)
            string archiveDir = Path.Combine("wwwroot", "ReceiptArchive");
            Directory.CreateDirectory(archiveDir);
            string pdfPath = Path.Combine(archiveDir, $"Receipt_{request.Id}.pdf");
            // TODO: Use SprintPdfArchive for actual PDF generation
            await File.WriteAllTextAsync(pdfPath, $"PDF for ServiceRequest {request.Id}"); // Placeholder
            request.FinalizedPDFPath = pdfPath;
            invoice.ArchivedAt = DateTime.UtcNow;
            invoice.DownloadUrl = $"/ReceiptArchive/Receipt_{request.Id}.pdf";
            _db.SaveChanges();
            return pdfPath;
        }
        public bool VerifySignedDocsExist(int serviceRequestId)
        {
            var request = _db.ServiceRequests.Find(serviceRequestId);
            if (request == null) return false;
            var completedDate = request.CompletedDate.HasValue ? request.CompletedDate.Value.Date : (DateTime?)null;
            BillingInvoiceRecord? invoice = null;
            if (completedDate.HasValue)
            {
                invoice = _db.BillingInvoiceRecords.FirstOrDefault(i => i.CustomerEmail == request.Email && i.InvoiceDate.Date == completedDate.Value);
            }
            if (invoice == null) return false;
            return !string.IsNullOrEmpty(invoice.CustomerSignaturePath) && File.Exists(request.FinalizedPDFPath ?? "");
        }
        public string GetArchivePath(int serviceRequestId)
        {
            var request = _db.ServiceRequests.Find(serviceRequestId);
            return request?.FinalizedPDFPath ?? string.Empty;
        }
    }
}
