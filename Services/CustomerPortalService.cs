using MVP_Core.Data;
using MVP_Core.Data.Models;
using BillingInvoiceRecordModel = MVP_Core.Data.Models.BillingInvoiceRecord;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    /// <summary>
    /// Aggregates customer portal data: service requests, loyalty points, documents, etc.
    /// </summary>
    public class CustomerPortalService
    {
        private readonly ApplicationDbContext _db;
        public CustomerPortalService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<ServiceRequest> GetServiceRequests(string email)
        {
            return _db.ServiceRequests.Where(r => r.Email == email).OrderByDescending(r => r.CreatedAt).ToList();
        }
        public List<LoyaltyPointTransaction> GetLoyaltyTransactions(string email)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null) return new List<LoyaltyPointTransaction>();
            return _db.LoyaltyPointTransactions.Where(t => t.CustomerId == customer.Id).OrderByDescending(t => t.Timestamp).ToList();
        }
        public List<CustomerReview> GetReviews(string email)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null) return new List<CustomerReview>();
            return _db.CustomerReviews.Where(r => r.CustomerId == customer.Id).OrderByDescending(r => r.SubmittedAt).ToList();
        }
        public List<RewardTier> GetRewardTiers()
        {
            return _db.RewardTiers.Where(t => t.IsActive).OrderBy(t => t.PointsRequired).ToList();
        }
        public List<BillingInvoiceRecordModel> GetDocuments(string email)
        {
            return _db.Set<BillingInvoiceRecordModel>().Where(d => d.CustomerEmail == email).OrderByDescending(d => d.InvoiceDate).ToList();
        }
        public Customer? GetCustomer(string email)
        {
            return _db.Customers.FirstOrDefault(c => c.Email == email);
        }
    }
}
