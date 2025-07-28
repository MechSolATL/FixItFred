// Sprint 85.4 — Coaching UI Enhancements + Export
using MVP_Core.Models;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public static class CsvExportHelper
    {
        // Sprint 85.4 — Coaching UI Enhancements + Export
        public static string ExportCoachingLogs(List<CoachingLogEntry> entries)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Date,TechnicianId,Supervisor,Category,Note");
            foreach (var e in entries)
            {
                sb.AppendLine($"{e.Timestamp:yyyy-MM-dd},{e.TechnicianId},\"{e.SupervisorName}\",\"{e.Category}\",\"{e.CoachingNote.Replace("\"", "''")}\"");
            }
            return sb.ToString();
        }
    }
}
