using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TrustCascadeLog
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int InitialAssigneeId { get; set; }
        public int FinalResolverId { get; set; }
        public int HopCount { get; set; }
        public double TrustDecay { get; set; }
        public string DelegationChain { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
