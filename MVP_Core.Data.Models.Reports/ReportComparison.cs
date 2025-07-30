// Sprint92_Fix_GroupB — Corrected syntax & namespace for comparison model

using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Data.Models.Reports
{
    public class ReportComparison
    {
        public List<Guid> TechnicianIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Dictionary<Guid, string> ComparisonData { get; set; }
    }
}
