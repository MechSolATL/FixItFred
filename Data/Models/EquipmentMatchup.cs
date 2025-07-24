// ===============================
// File: Data/Models/EquipmentMatchup.cs
// ===============================

namespace MVP_Core.Data.Models
{
    public class EquipmentMatchup
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string EquipmentType { get; set; } = string.Empty; // e.g., "Mini Split Heat Pump"

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty; // e.g., "Multi-Position", "Ducted Vertical"

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ModelNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CoolingBTU { get; set; } = string.Empty;

        [MaxLength(500)]
        public string HeatingBTU { get; set; } = string.Empty;

        public string Overview { get; set; } = string.Empty;

        public string BenefitsFeatures { get; set; } = string.Empty;

        public string Components { get; set; } = string.Empty; // Indoor, Outdoor, Remote

        [MaxLength(500)]
        public string FinancingUrl { get; set; } = string.Empty;

        public bool AHRI_Certified { get; set; } = true;

        public bool Supports_DualFuel { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
