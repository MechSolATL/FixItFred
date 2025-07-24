namespace MVP_Core.Data.Models
{
    public class BillingInvoiceRecord
    {
        public int Id { get; set; }
        public string ExternalInvoiceId { get; set; } = string.Empty;
        public string RealmId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal AmountDue { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty; // e.g., Open, Paid, Overdue
        public string LinkUrl { get; set; } = string.Empty;
        public DateTime RetrievedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
