// ===============================
// File: Data/Models/HeatPumpMatchup.cs
// ===============================

namespace MVP_Core.Data.Models
{
    public class HeatPumpMatchup
    {
        /// <summary>
        /// The unique identifier for the heat pump matchup.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The brand of the heat pump system.
        /// </summary>
        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// The type of system (e.g., "Unitary", "Heat Pump Only").
        /// </summary>
        [Required, MaxLength(100)]
        public string SystemType { get; set; } = string.Empty; // e.g., "Unitary", "Heat Pump Only"

        /// <summary>
        /// The model of the A-coil unit.
        /// </summary>
        [Required, MaxLength(100)]
        public string ACoilModel { get; set; } = string.Empty;

        /// <summary>
        /// The tonnage of the A-coil unit.
        /// </summary>
        [MaxLength(50)]
        public string ACoilTonnage { get; set; } = string.Empty;

        /// <summary>
        /// The model of the outdoor unit.
        /// </summary>
        [Required, MaxLength(100)]
        public string OutdoorUnitModel { get; set; } = string.Empty;

        /// <summary>
        /// The tonnage of the outdoor unit.
        /// </summary>
        [MaxLength(50)]
        public string OutdoorUnitTonnage { get; set; } = string.Empty;

        /// <summary>
        /// The model of the furnace unit.
        /// </summary>
        [MaxLength(100)]
        public string FurnaceModel { get; set; } = string.Empty;

        /// <summary>
        /// The BTU rating of the furnace unit.
        /// </summary>
        [MaxLength(50)]
        public string FurnaceBTU { get; set; } = string.Empty;

        /// <summary>
        /// The SEER rating of the heat pump system.
        /// </summary>
        [MaxLength(50)]
        public string SEERRating { get; set; } = string.Empty;

        /// <summary>
        /// The AHRI certification code for the system.
        /// </summary>
        [MaxLength(50)]
        public string AHRICode { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes about the heat pump matchup.
        /// </summary>
        [MaxLength(255)]
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the heat pump matchup was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
