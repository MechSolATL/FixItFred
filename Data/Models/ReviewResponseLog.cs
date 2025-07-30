using System;

namespace MVP_Core.Data.Models
{
    public class ReviewResponseLog
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Platform { get; set; }
        public string Review { get; set; }
        public string Response { get; set; }
        public string TechName { get; set; }
        public string ZipCode { get; set; }
        public string ServiceAddress { get; set; }
        public bool IsFlagged { get; set; }
        public bool ResponseSuccess { get; set; }
        public DateTime Timestamp { get; set; }
    }
}