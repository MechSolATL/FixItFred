using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;

namespace Data.Models
{
    public class Technician
    {
        /// <summary>
        /// The unique identifier for the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The full name of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        /// <summary>
        /// The phone number of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Indicates whether the technician is active.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// The specialty of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [MaxLength(100)]
        public string? Specialty { get; set; }

        /// <summary>
        /// The current number of jobs assigned to the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int CurrentJobCount { get; set; } = 0;

        /// <summary>
        /// The maximum number of jobs the technician can handle.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int MaxJobCapacity { get; set; } = 5;

        /// <summary>
        /// The birthday of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// The employment start date of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public DateTime? EmploymentDate { get; set; }

        /// <summary>
        /// The URL of the technician's photo.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// A JSON array of badge names or objects associated with the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public string? Badges { get; set; }

        /// <summary>
        /// The latitude of the technician's current location.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude of the technician's current location.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public double? Longitude { get; set; }

        /// <summary>
        /// The dispatch score of the technician, indicating performance.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int? DispatchScore { get; set; }

        /// <summary>
        /// Comma-separated skill tags for technician specialization.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [MaxLength(1000)]
        public string? SkillTags { get; set; }

        /// <summary>
        /// The hourly rate for the technician's pay calculation.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public decimal? HourlyRate { get; set; }

        /// <summary>
        /// Indicates whether the technician is flagged as a second-chance tech.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public bool IsSecondChance { get; set; } = false;

        /// <summary>
        /// Indicates whether the technician requires supervision.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public bool RequiresSupervision { get; set; } = false;

        /// <summary>
        /// The onboarding status of the technician (e.g., Pending, Approved, Blocked).
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [MaxLength(50)]
        public string? OnboardingStatus { get; set; }

        /// <summary>
        /// The leaderboard tier level of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int TierLevel { get; set; } = 0;

        /// <summary>
        /// The total points accumulated by the technician on the leaderboard.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int TotalPoints { get; set; } = 0;

        /// <summary>
        /// The city where the technician is based.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// The full name of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public string Name => FullName;

        /// <summary>
        /// The trust score of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int TrustScore { get; set; } = 100;

        /// <summary>
        /// The heat score of the technician, used for map overlays.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int HeatScore { get; set; } = 100;

        /// <summary>
        /// The zip code of the technician's location.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public string ZipCode { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the technician was last reviewed.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public DateTime? LastReviewedAt { get; set; }

        /// <summary>
        /// The last known heat score of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public int? LastKnownHeatScore { get; set; }

        /// <summary>
        /// The nickname of the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        [MaxLength(40)]
        public string? Nickname { get; set; }

        /// <summary>
        /// Indicates whether the technician's nickname is approved.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public bool NicknameApproved { get; set; }

        /// <summary>
        /// Indicates whether banter mode is enabled for the technician.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public bool EnableBanterMode { get; set; }

        /// <summary>
        /// The reputation score of the technician within the Patch module.
        /// </summary>
        // AutoDoc by FixItFred – Sprint92_ModelDocs
        public double PatchReputationScore { get; set; } = 0;
    }
}
