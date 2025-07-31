using Data;
using Data.Models;
using System;
using System.Linq;

namespace Services.Admin
{
    public class AccountabilityEngineService
    {
        private readonly ApplicationDbContext _db;
        public AccountabilityEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        public AccountabilityDelayLog LogResolutionDelay(string employeeId, int delayDurationHours, string? responsiblePartyId, string? notes)
        {
            var log = new AccountabilityDelayLog
            {
                EmployeeId = employeeId,
                CreatedAt = DateTime.UtcNow,
                DelayDurationHours = delayDurationHours,
                ResponsiblePartyId = responsiblePartyId,
                Notes = notes,
                IsEscalated = delayDurationHours > 48,
                IsFlagged = delayDurationHours > 48,
                LastUpdated = DateTime.UtcNow
            };
            _db.AccountabilityDelayLogs.Add(log);
            _db.SaveChanges();
            return log;
        }

        public void MarkResolved(int logId, string reviewerComment)
        {
            var log = _db.AccountabilityDelayLogs.FirstOrDefault(x => x.Id == logId);
            if (log != null)
            {
                log.ResolutionTimestamp = DateTime.UtcNow;
                log.ReviewerComment = reviewerComment;
                log.IsEscalated = false;
                log.IsFlagged = false;
                log.LastUpdated = DateTime.UtcNow;
                _db.SaveChanges();
            }
        }
    }
}
