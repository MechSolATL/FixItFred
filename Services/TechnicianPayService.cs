using System;
using System.Collections.Generic;
using System.Linq;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Services
{
    public class TechnicianPayService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianPayService(ApplicationDbContext db)
        {
            _db = db;
        }
        public TechnicianPayRecord CalculatePay(int technicianId, DateTime periodStart, DateTime periodEnd)
        {
            // Example: Calculate pay based on job logs
            var jobs = _db.ScheduleQueues.Where(q => q.TechnicianId == technicianId && q.ScheduledTime >= periodStart && q.ScheduledTime <= periodEnd).ToList();
            decimal hoursWorked = jobs.Sum(j => (decimal)(j.EstimatedDurationHours ?? 0));
            decimal commissionEarned = jobs.Sum(j => (decimal)(j.CommissionAmount ?? 0));
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId);
            decimal hourlyRate = tech?.HourlyRate ?? 0;
            string payType = "Commission";
            decimal totalPay = commissionEarned;
            // Sprint 51.1: Second-chance techs default to salary model unless overridden
            if (tech != null && tech.IsSecondChance)
            {
                payType = "Salary";
                // Example salary logic: base salary + commission
                decimal baseSalary = 1000; // Could be configurable
                totalPay = baseSalary + commissionEarned;
                if (hourlyRate > 0)
                {
                    payType = "Hourly";
                    totalPay = hoursWorked * hourlyRate;
                }
            }
            else if (hourlyRate > 0)
            {
                payType = "Hourly";
                totalPay = hoursWorked * hourlyRate;
            }
            return new TechnicianPayRecord
            {
                TechnicianId = technicianId,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                HoursWorked = hoursWorked,
                CommissionEarned = commissionEarned,
                HourlyRate = hourlyRate,
                TotalPay = totalPay,
                PayType = payType,
                CreatedAt = DateTime.UtcNow
            };
        }
        public List<TechnicianPayRecord> GetPayHistory(int technicianId)
        {
            return _db.TechnicianPayRecords.Where(p => p.TechnicianId == technicianId).OrderByDescending(p => p.PeriodEnd).ToList();
        }
    }
}