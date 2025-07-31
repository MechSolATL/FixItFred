using System.Collections.Generic;
using System.Linq;
using System;
using Data;
using Data.Models;

namespace Services
{
    /// <summary>
    /// Service for managing skill tracks and progress for technicians.
    /// </summary>
    public class SkillsTrackerService
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillsTrackerService"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        public SkillsTrackerService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves the skill tracks assigned to a specific technician.
        /// </summary>
        /// <param name="technicianId">The ID of the technician.</param>
        /// <returns>A list of assigned skill tracks.</returns>
        public List<SkillTrack> GetAssignedTracks(int technicianId)
        {
            return _db.SkillTracks.Where(t => t.AssignedTo.Contains(technicianId)).ToList();
        }

        /// <summary>
        /// Retrieves the progress of skill tracks for a specific technician.
        /// </summary>
        /// <param name="technicianId">The ID of the technician.</param>
        /// <returns>A list of skill progress entries.</returns>
        public List<SkillProgress> GetProgressForTechnician(int technicianId)
        {
            return _db.SkillProgresses.Where(p => p.TechnicianId == technicianId).ToList();
        }

        /// <summary>
        /// Retrieves all available skill tracks.
        /// </summary>
        /// <returns>A list of all skill tracks.</returns>
        public List<SkillTrack> GetAllTracks()
        {
            return _db.SkillTracks.ToList();
        }

        /// <summary>
        /// Retrieves all technicians.
        /// </summary>
        /// <returns>A list of all technicians.</returns>
        public List<Technician> GetAllTechnicians()
        {
            return _db.Technicians.ToList();
        }

        /// <summary>
        /// Assigns a skill track to a technician.
        /// </summary>
        /// <param name="technicianId">The ID of the technician.</param>
        /// <param name="skillTrackId">The ID of the skill track.</param>
        /// <returns>True if the assignment was successful; otherwise, false.</returns>
        public bool AssignTrack(int technicianId, int skillTrackId)
        {
            var track = _db.SkillTracks.FirstOrDefault(t => t.Id == skillTrackId);
            if (track == null) return false;
            if (!track.AssignedTo.Contains(technicianId)) track.AssignedTo.Add(technicianId);
            _db.SkillProgresses.Add(new SkillProgress { TechnicianId = technicianId, SkillTrackId = skillTrackId, Status = "Assigned" });
            _db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Marks a skill track as completed for a technician.
        /// </summary>
        /// <param name="technicianId">The ID of the technician.</param>
        /// <param name="skillTrackId">The ID of the skill track.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public bool MarkTrackCompleted(int technicianId, int skillTrackId)
        {
            var progress = _db.SkillProgresses.FirstOrDefault(p => p.TechnicianId == technicianId && p.SkillTrackId == skillTrackId);
            if (progress == null) return false;
            progress.Status = "Completed";
            progress.CompletedDate = DateTime.UtcNow;
            _db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Determines whether a technician is eligible for a badge based on completed skill tracks.
        /// </summary>
        /// <param name="technicianId">The ID of the technician.</param>
        /// <returns>True if the technician is eligible for a badge; otherwise, false.</returns>
        public bool IsEligibleForBadge(int technicianId)
        {
            var requiredTracks = _db.SkillTracks.Where(t => t.IsRequired && t.AssignedTo.Contains(technicianId)).Select(t => t.Id).ToList();
            var completedTracks = _db.SkillProgresses.Where(p => p.TechnicianId == technicianId && p.Status == "Completed").Select(p => p.SkillTrackId).ToList();
            return requiredTracks.All(tid => completedTracks.Contains(tid));
        }
    }
}
