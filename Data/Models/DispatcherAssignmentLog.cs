using System;

namespace MVP_Core.Data.Models
{
    public class DispatcherAssignmentLog
    {
        public int Id { get; set; }
        public int DispatcherId { get; set; }
        public int TechnicianId { get; set; }
        public int ServiceRequestId { get; set; }
        public DateTime AssignedAt { get; set; }
        public double DistanceToJob { get; set; }
        public int JobPriority { get; set; }
        public decimal JobValue { get; set; }
        public bool AutoAssigned { get; set; }
        public bool FlaggedForReview { get; set; }
        public string? JustificationJson { get; set; }
    }

    public class FavoritismAlertLog
    {
        public int Id { get; set; }
        public int DispatcherId { get; set; }
        public int TechnicianId { get; set; }
        public double PatternScore { get; set; }
        public DateTime FlaggedAt { get; set; }
        public bool AdminReviewed { get; set; }
        public string? ResolutionNotes { get; set; }
    }
}
