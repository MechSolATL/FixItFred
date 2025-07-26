// Sprint 26.6 Patch Log: Added commission and financing calculation methods for admin UI and backend.
using MVP_Core.Data.Models;
using System;

namespace MVP_Core.Services
{
    public class CommissionFinancingService
    {
        // Sprint 26.6: Calculate technician commission (e.g., 10% of job value)
        public decimal CalculateCommission(decimal jobValue, decimal rate = 0.10m)
        {
            return Math.Round(jobValue * rate, 2);
        }

        // Sprint 26.6: Calculate monthly payment for financing
        public decimal CalculateMonthlyPayment(decimal amount, decimal apr, int termMonths)
        {
            if (termMonths <= 0 || apr < 0) return 0;
            var monthlyRate = apr / 12 / 100;
            if (monthlyRate == 0) return Math.Round(amount / termMonths, 2);
            var payment = amount * (monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, termMonths)) /
                ((decimal)Math.Pow(1 + (double)monthlyRate, termMonths) - 1);
            return Math.Round(payment, 2);
        }
    }
}
