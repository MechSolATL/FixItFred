using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class StorageGrowthSnapshot
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double UsageMB { get; set; } // Added for digest summary
        // ...other properties...
    }
}
