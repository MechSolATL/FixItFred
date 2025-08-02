// Sprint 91.17 - TroubleshootingBrain
using System;

namespace Data.Models
{
    /// <summary>
    /// Represents a log entry for troubleshooting attempts made by technicians.
    /// </summary>
    public class TroubleshootingAttemptLog
    {
        /// <summary>
        /// Gets or sets the unique identifier for the troubleshooting attempt log.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the technician who made the attempt.
        /// </summary>
        public Guid TechId { get; set; }

        /// <summary>
        /// Gets or sets the input prompt used during the troubleshooting attempt.
        /// </summary>
        public string PromptInput { get; set; }

        /// <summary>
        /// Gets or sets the suggested fix provided during the troubleshooting attempt.
        /// </summary>
        public string SuggestedFix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the troubleshooting attempt was successful.
        /// </summary>
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the notes provided by the technician during the troubleshooting attempt.
        /// </summary>
        public string TechNotes { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the troubleshooting attempt.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Represents a known fix for specific error codes and equipment types.
    /// </summary>
    public class KnownFix
    {
        /// <summary>
        /// Gets or sets the unique identifier for the known fix.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the error code associated with the known fix.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the type of equipment associated with the known fix.
        /// </summary>
        public string EquipmentType { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer of the equipment associated with the known fix.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the common fix for the error code and equipment type.
        /// </summary>
        public string CommonFix { get; set; }

        /// <summary>
        /// Gets or sets the count of successful applications of the known fix.
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last confirmation of the known fix.
        /// </summary>
        public DateTime LastConfirmed { get; set; }
    }
}
