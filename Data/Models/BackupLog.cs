namespace MVP_Core.Data.Models
{
    public class BackupLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(20)]
        public string BackupType { get; set; } = "Full"; // Full, Differential, Log

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Success"; // Success, Failed, Warning

        [MaxLength(4000)]
        public string? Message { get; set; } // Optional error/success message

        public int DurationSeconds { get; set; } = 0; // How long backup took

        public int BackupSizeMB { get; set; } = 0;    // Size of backup file in MB

        [MaxLength(100)]
        public string? SourceServer { get; set; } // Optional: Server name or ID
    }
}
