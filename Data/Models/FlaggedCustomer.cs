namespace MVP_Core.Data.Models
{
    public class FlaggedCustomer
    {
        /// <summary>
        /// The unique identifier for the flagged customer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the flagged customer.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// The phone number of the flagged customer.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// The user or system that flagged the customer.
        /// </summary>
        public string FlaggedBy { get; set; } = string.Empty;

        /// <summary>
        /// The reason for flagging the customer.
        /// </summary>
        public string FlaggedReason { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the flagged customer.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the customer was flagged.
        /// </summary>
        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The user agent information associated with the flagged customer.
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;
    }
}
