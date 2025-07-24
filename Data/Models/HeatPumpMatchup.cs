// ===============================
// File: Data/Models/HeatPumpMatchup.cs
// ===============================

namespace MVP_Core.Data.Models
{
    public class HeatPumpMatchup
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string SystemType { get; set; } = string.Empty; // e.g., "Unitary", "Heat Pump Only"

        [Required, MaxLength(100)]
        public string ACoilModel { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ACoilTonnage { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string OutdoorUnitModel { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OutdoorUnitTonnage { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FurnaceModel { get; set; } = string.Empty;

        [MaxLength(50)]
        public string FurnaceBTU { get; set; } = string.Empty;

        [MaxLength(50)]
        public string SEERRating { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AHRICode { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
