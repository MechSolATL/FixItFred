using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.UI
{
    public class TechnicianComparisonReportRequestViewModel
    {
        [Required]
        public List<Guid> TechIds { get; set; } = new();

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }
}
