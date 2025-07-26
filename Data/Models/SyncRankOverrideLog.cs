using System;
using System.ComponentModel.DataAnnotations;
using MVP_Core.Data.Models;

namespace MVP_Core.Data.Models
{
    public class SyncRankOverrideLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public SyncRankLevel PreviousRank { get; set; }
        public SyncRankLevel NewRank { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
