namespace MVP_Core.Data.Models
{
    public class PendingEntry
    {
        /// <summary>
        /// The unique identifier for the pending entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The type of the pending entry.
        /// </summary>
        public string TryType { get; set; } = string.Empty;

        /// <summary>
        /// The email address associated with the pending entry.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The phone number associated with the pending entry.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// The status of the pending entry (e.g., New, InProgress, Completed).
        /// </summary>
        public string Status { get; set; } = "New";

        /// <summary>
        /// The timestamp when the pending entry was submitted.
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
