using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class TechnicianPayRecord
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal CommissionEarned { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalPay { get; set; }
        public string PayType { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
        public string? BonusNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}