using System;

namespace MVP_Core.Data.Models
{
    public class ReviewResponseLog
    {
        /// <summary>
        /// The unique identifier for the review response log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the customer who submitted the review.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// The platform where the review was submitted.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// The content of the customer's review.
        /// </summary>
        public string Review { get; set; }

        /// <summary>
        /// The response to the customer's review.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// The name of the technician associated with the review.
        /// </summary>
        public string TechName { get; set; }

        /// <summary>
        /// The zip code of the service location.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// The address of the service location.
        /// </summary>
        public string ServiceAddress { get; set; }

        /// <summary>
        /// Indicates whether the review is flagged.
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Indicates whether the response to the review was successful.
        /// </summary>
        public bool ResponseSuccess { get; set; }

        /// <summary>
        /// The timestamp when the review response log entry was created.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}