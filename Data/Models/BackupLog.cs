namespace MVP_Core.Data.Models
{
    public class BackupLog
    {
        /// <summary>
        /// The unique identifier for the backup log.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The date and time when the backup was performed.
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// The type of backup (e.g., Full, Differential, Log).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string BackupType { get; set; } = "Full";

        /// <summary>
        /// The status of the backup (e.g., Success, Failed, Warning).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Success";

        /// <summary>
        /// An optional message providing details about the backup.
        /// </summary>
        [MaxLength(4000)]
        public string? Message { get; set; }

        /// <summary>
        /// The duration of the backup in seconds.
        /// </summary>
        public int DurationSeconds { get; set; } = 0;

        /// <summary>
        /// The size of the backup file in megabytes.
        /// </summary>
        public int BackupSizeMB { get; set; } = 0;

        /// <summary>
        /// The name or ID of the source server for the backup.
        /// </summary>
        [MaxLength(100)]
        public string? SourceServer { get; set; }
    }
}
