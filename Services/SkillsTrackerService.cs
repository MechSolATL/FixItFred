using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    public class SkillsTrackerService
    {
        private readonly MVP_Core.Data.ApplicationDbContext _db;
        public SkillsTrackerService(MVP_Core.Data.ApplicationDbContext db)
        {
            _db = db;
        }
        public List<MVP_Core.Data.Models.SkillTrack> GetAssignedTracks(int technicianId)
        {
            return _db.SkillTracks.Where(t => t.AssignedTo.Contains(technicianId)).ToList();
        }
        public List<MVP_Core.Data.Models.SkillProgress> GetProgressForTechnician(int technicianId)
        {
            return _db.SkillProgresses.Where(p => p.TechnicianId == technicianId).ToList();
        }
        public List<MVP_Core.Data.Models.SkillTrack> GetAllTracks()
        {
            return _db.SkillTracks.ToList();
        }
        public List<MVP_Core.Data.Models.Technician> GetAllTechnicians()
        {
            return _db.Technicians.ToList();
        }
        public bool AssignTrack(int technicianId, int skillTrackId)
        {
            var track = _db.SkillTracks.FirstOrDefault(t => t.Id == skillTrackId);
            if (track == null) return false;
            if (!track.AssignedTo.Contains(technicianId)) track.AssignedTo.Add(technicianId);
            _db.SkillProgresses.Add(new MVP_Core.Data.Models.SkillProgress { TechnicianId = technicianId, SkillTrackId = skillTrackId, Status = "Assigned" });
            _db.SaveChanges();
            return true;
        }
        public bool MarkTrackCompleted(int technicianId, int skillTrackId)
        {
            var progress = _db.SkillProgresses.FirstOrDefault(p => p.TechnicianId == technicianId && p.SkillTrackId == skillTrackId);
            if (progress == null) return false;
            progress.Status = "Completed";
            progress.CompletedDate = System.DateTime.UtcNow;
            _db.SaveChanges();
            return true;
        }
        public bool IsEligibleForBadge(int technicianId)
        {
            var requiredTracks = _db.SkillTracks.Where(t => t.IsRequired && t.AssignedTo.Contains(technicianId)).Select(t => t.Id).ToList();
            var completedTracks = _db.SkillProgresses.Where(p => p.TechnicianId == technicianId && p.Status == "Completed").Select(p => p.SkillTrackId).ToList();
            return requiredTracks.All(tid => completedTracks.Contains(tid));
        }
    }
}
