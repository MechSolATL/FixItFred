using System;

namespace MVP_Core.Services.CommissionCalculator
{
    public interface ICommissionCalculatorService
    {
        decimal CalculateCommission(decimal saleAmount, string technicianRank);
    }

    public class CommissionCalculatorService : ICommissionCalculatorService
    {
        public decimal CalculateCommission(decimal saleAmount, string technicianRank)
        {
            return technicianRank switch
            {
                "Platinum" => saleAmount * 0.20m,
                "Gold" => saleAmount * 0.15m,
                "Silver" => saleAmount * 0.10m,
                _ => saleAmount * 0.05m
            };
        }
    }
}