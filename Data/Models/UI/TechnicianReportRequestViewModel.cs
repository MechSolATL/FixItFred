using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.UI
{
    public class TechnicianReportRequestViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }
}
