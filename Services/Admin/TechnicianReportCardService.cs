using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class TechnicianReportCardService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianReportCardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<TechnicianReportCard> GetAllReportCards()
        {
            var technicians = _db.Technicians.ToList();
            var cards = new List<TechnicianReportCard>();
            foreach (var tech in technicians)
            {
                var lateLogs = _db.LateClockInLogs.Where(x => x.TechnicianId == tech.Id).ToList();
                var idleLogs = _db.IdleSessionMonitorLogs.Where(x => x.TechnicianId == tech.Id).ToList();
                var egoLog = _db.EgoVectorLogs.OrderByDescending(x => x.WeekStart).FirstOrDefault(x => x.ManagerId == tech.Id);
                var confidenceLog = _db.EmployeeConfidenceDecayLogs.OrderByDescending(x => x.WeekStart).FirstOrDefault(x => x.ManagerId == tech.Id);
                var trustLog = _db.TechnicianTrustLogs.OrderByDescending(x => x.Id).FirstOrDefault(x => x.TechnicianId == tech.Id);

                cards.Add(new TechnicianReportCard
                {
                    TechnicianId = tech.Id,
                    TechnicianName = tech.Name,
                    LateClockInCount = lateLogs.Count,
                    SevereLateClockInCount = lateLogs.Count(x => x.Severity == "Severe"),
                    IdleMinutesWeek = idleLogs.Where(x => x.IdleStartTime > DateTime.UtcNow.AddDays(-7)).Sum(x => x.IdleMinutes),
                    EgoInfluenceScore = egoLog?.EgoInfluenceScore ?? 0,
                    ConfidenceShift = confidenceLog?.ConfidenceShift ?? 0,
                    PulseScore = confidenceLog?.PulseScore ?? 0,
                    TrustScore = trustLog != null ? (double)trustLog.TrustScore : 0,
                    Notes = confidenceLog?.Notes
                });
            }
            return cards;
        }
    }
}
