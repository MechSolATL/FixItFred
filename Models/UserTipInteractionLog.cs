using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// Logs user interactions with tips and guidance elements for adaptive learning
    /// Feeds into UserPerformanceLevelEngine for skill assessment
    /// </summary>
    public class UserTipInteractionLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User identifier who interacted with the tip
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Unique identifier for the tip or guidance element
        /// </summary>
        public string TipId { get; set; } = string.Empty;

        /// <summary>
        /// Type of interaction: "viewed", "dismissed", "suppressed", "acted_upon"
        /// </summary>
        public string InteractionType { get; set; } = string.Empty;

        /// <summary>
        /// Task or workflow context where the tip was displayed
        /// </summary>
        public string TaskContext { get; set; } = string.Empty;

        /// <summary>
        /// Page or component where the interaction occurred
        /// </summary>
        public string PageContext { get; set; } = string.Empty;

        /// <summary>
        /// Time taken to interact with the tip (in milliseconds)
        /// </summary>
        public int InteractionDurationMs { get; set; }

        /// <summary>
        /// Whether the user completed the suggested action
        /// </summary>
        public bool ActionCompleted { get; set; }

        /// <summary>
        /// Number of times this user has seen this specific tip
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// Session identifier for grouping related interactions
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the interaction occurred
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Additional metadata about the interaction context
        /// </summary>
        public string Metadata { get; set; } = string.Empty;

        /// <summary>
        /// Whether this interaction indicates muscle memory development
        /// </summary>
        public bool IndicatesMuscleMemory { get; set; }

        /// <summary>
        /// User's current performance level when interaction occurred
        /// </summary>
        public string UserPerformanceLevel { get; set; } = "Beginner";
    }
}