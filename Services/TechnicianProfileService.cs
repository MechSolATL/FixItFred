using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MVP_Core.Services
{
    public class PatchIdentityUpdateResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public bool Flagged { get; set; }
    }

    public interface ITechnicianProfileService
    {
        Task<TechnicianProfileDto?> GetProfileAsync(int techId);
        Task<TechnicianAnalyticsDto> GetAnalyticsAsync(int techId, DateRange range);
        Task<PatchIdentityUpdateResult> UpdatePatchIdentityAsync(int techId, string? nickname, bool enableBanterMode);
        // ...other methods as needed...
    }

    public class TechnicianHeatmapCell
    {
        public int DayOfWeek { get; set; } // 0=Sunday, 6=Saturday
        public int HourOfDay { get; set; } // 0-23
        public int Jobs { get; set; }
        public int Delays { get; set; }
        public int Callbacks { get; set; }
    }

    public class TechnicianProfileService : ITechnicianProfileService
    {
        private readonly ApplicationDbContext _db;
        private static readonly string[] BannedWords = new[]
        {
            // Add more as needed for PROS compliance
            "damn", "hell", "shit", "fuck", "bastard", "idiot", "dumb", "stupid", "ass", "bitch", "crap", "fool", "moron"
        };
        private const string BanterFlagSessionKey = "PatchBanterFlagCount";
        public TechnicianProfileService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<TechnicianProfileDto?> GetProfileAsync(int techId)
        {
            var tech = await _db.Technicians.FindAsync(techId);
            if (tech == null) return null;

            // Aggregate stats
            var completedJobs = await _db.ServiceRequests.CountAsync(r => r.AssignedTechnicianId == techId && r.Status == "Complete");
            var totalJobs = await _db.ServiceRequests.CountAsync(r => r.AssignedTechnicianId == techId);
            var callbacks = await _db.ServiceRequests.CountAsync(r => r.AssignedTechnicianId == techId && r.NeedsFollowUp);
            var reviews = await _db.ProfileReviews.Where(r => r.Username == tech.FullName).ToListAsync();
            double avgReview = reviews.Count > 0 ? reviews.Average(r => r.ReviewNotes != null ? 1 : 0) : 0; // Placeholder
            var skills = await _db.TechnicianSkillMaps
                .Where(m => m.TechnicianId == techId && m.Skill != null)
                .Select(m => m.Skill!.Name)
                .ToListAsync();

            return new TechnicianProfileDto
            {
                Id = tech.Id,
                FullName = tech.FullName,
                Email = tech.Email,
                Phone = tech.Phone,
                Specialty = tech.Specialty,
                IsActive = tech.IsActive,
                Birthday = tech.Birthday,
                EmploymentDate = tech.EmploymentDate,
                PhotoUrl = tech.PhotoUrl,
                Badges = tech.Badges,
                CompletedJobs = completedJobs,
                Callbacks = callbacks,
                CloseRate = totalJobs > 0 ? (completedJobs / (double)totalJobs) : 0,
                AvgReviewScore = avgReview,
                ReviewCount = reviews.Count,
                Skills = skills,
                Nickname = tech.Nickname,
                NicknameApproved = tech.NicknameApproved,
                EnableBanterMode = tech.EnableBanterMode
            };
        }

        public async Task<PatchIdentityUpdateResult> UpdatePatchIdentityAsync(int techId, string? nickname, bool enableBanterMode)
        {
            var result = new PatchIdentityUpdateResult();
            var tech = await _db.Technicians.FindAsync(techId);
            if (tech == null)
            {
                result.Success = false;
                result.Message = "Technician not found.";
                return result;
            }
            // Nickname compliance check
            if (!string.IsNullOrWhiteSpace(nickname))
            {
                var lower = nickname.ToLowerInvariant();
                foreach (var banned in BannedWords)
                {
                    if (Regex.IsMatch(lower, $"\\b{Regex.Escape(banned)}\\b"))
                    {
                        result.Success = false;
                        result.Message = "Whoa, whoa, whoa. Patch don’t talk like that. Clean it up.";
                        result.Flagged = true;
                        return result;
                    }
                }
                if (nickname.Length > 40)
                {
                    result.Success = false;
                    result.Message = "Nickname too long (max 40 chars).";
                    result.Flagged = true;
                    return result;
                }
            }
            tech.Nickname = nickname;
            tech.NicknameApproved = !string.IsNullOrWhiteSpace(nickname) && !result.Flagged;
            tech.EnableBanterMode = enableBanterMode;
            await _db.SaveChangesAsync();
            result.Success = true;
            return result;
        }

        public async Task<byte[]> ExportCsvAsync(int techId)
        {
            var profile = await GetProfileAsync(techId);
            if (profile == null) return Array.Empty<byte>();
            var sb = new StringBuilder();
            sb.AppendLine("Field,Value");
            sb.AppendLine($"FullName,{profile.FullName}");
            sb.AppendLine($"Email,{profile.Email}");
            sb.AppendLine($"Phone,{profile.Phone}");
            sb.AppendLine($"Specialty,{profile.Specialty}");
            sb.AppendLine($"Birthday,{profile.Birthday}");
            sb.AppendLine($"EmploymentDate,{profile.EmploymentDate}");
            sb.AppendLine($"CompletedJobs,{profile.CompletedJobs}");
            sb.AppendLine($"Callbacks,{profile.Callbacks}");
            sb.AppendLine($"CloseRate,{profile.CloseRate:P2}");
            sb.AppendLine($"AvgReviewScore,{profile.AvgReviewScore}");
            sb.AppendLine($"ReviewCount,{profile.ReviewCount}");
            sb.AppendLine($"Skills,{string.Join("; ", profile.Skills)}");
            sb.AppendLine($"Badges,{profile.Badges}");
            sb.AppendLine();
            // Job-level analytics
            sb.AppendLine("JobId,ServiceType,Status,ETA,ScheduledAt,ClosedAt,DelayMin");
            var jobs = await _db.ServiceRequests.Where(r => r.AssignedTechnicianId == techId).ToListAsync();
            foreach (var job in jobs)
            {
                var eta = job.ScheduledAt.HasValue && job.ClosedAt.HasValue ? (job.ClosedAt <= job.ScheduledAt ? "On Time" : "Delayed") : "N/A";
                var delay = job.ScheduledAt.HasValue && job.ClosedAt.HasValue && job.ClosedAt > job.ScheduledAt ? (job.ClosedAt - job.ScheduledAt)?.TotalMinutes.ToString("F1") : "0";
                sb.AppendLine($"{job.Id},{job.ServiceType},{job.Status},{eta},{job.ScheduledAt},{job.ClosedAt},{delay}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> ExportCsvAsync(List<int> techIds)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tech,JobId,ServiceType,Status,ETA,ScheduledAt,ClosedAt,DelayMin");
            foreach (var techId in techIds)
            {
                var profile = await GetProfileAsync(techId);
                if (profile == null) continue;
                var jobs = await _db.ServiceRequests.Where(r => r.AssignedTechnicianId == techId).ToListAsync();
                foreach (var job in jobs)
                {
                    var eta = job.ScheduledAt.HasValue && job.ClosedAt.HasValue ? (job.ClosedAt <= job.ScheduledAt ? "On Time" : "Delayed") : "N/A";
                    var delay = job.ScheduledAt.HasValue && job.ClosedAt.HasValue && job.ClosedAt > job.ScheduledAt ? (job.ClosedAt - job.ScheduledAt)?.TotalMinutes.ToString("F1") : "0";
                    sb.AppendLine($"{profile.FullName},{job.Id},{job.ServiceType},{job.Status},{eta},{job.ScheduledAt},{job.ClosedAt},{delay}");
                }
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> ExportPdfAsync(int techId)
        {
            // Stub: implement with DinkToPdf/iTextSharp in future
            await Task.CompletedTask;
            return Encoding.UTF8.GetBytes("PDF export not implemented yet.");
        }

        /// <summary>
        /// Returns analytics for a technician: close rate trends, callback trends, ETA success, and average delay.
        /// Adds predictive metrics and risk flagging.
        /// </summary>
        /// <param name="techId">Technician ID</param>
        /// <param name="range">Date range for analytics</param>
        /// <returns>TechnicianAnalyticsDto</returns>
        public async Task<TechnicianAnalyticsDto> GetAnalyticsAsync(int techId, DateRange range)
        {
            var requests = await _db.ServiceRequests
                .Where(r => r.AssignedTechnicianId == techId && r.CreatedAt >= range.Start && r.CreatedAt <= range.End)
                .ToListAsync();

            var (closeRates, callbacks, etaSuccess, avgDelay) = CalculateKpis(requests);

            // --- Predictive metrics (simple moving average/last period) ---
            // 7-day and 30-day close rate and callback rate forecasts
            var now = DateTime.UtcNow;
            var (closeRate7, closeRate30, callbackRate7, callbackRate30) = CalculateForecasts(requests, now);

            // --- Risk flagging ---
            // Thresholds (could be moved to config)
            const double CLOSE_RATE_THRESHOLD = 0.8;
            const double CALLBACK_RATE_THRESHOLD = 0.1;
            var (isAtRisk, riskFlags) = EvaluateRisk(closeRate7, closeRate30, callbackRate7, callbackRate30, CLOSE_RATE_THRESHOLD, CALLBACK_RATE_THRESHOLD);

            return new TechnicianAnalyticsDto
            {
                CloseRateTrends = closeRates,
                CallbackTrends = callbacks,
                EtaSuccessRate = etaSuccess,
                AverageDelayMinutes = avgDelay,
                CloseRateForecast7d = closeRate7,
                CloseRateForecast30d = closeRate30,
                CallbackRateForecast7d = callbackRate7,
                CallbackRateForecast30d = callbackRate30,
                IsAtRisk = isAtRisk,
                RiskFlags = riskFlags
            };
        }

        #region PredictiveLogicForUnitTests
        // Expose static/virtual for testability
        public static (double closeRate7, double closeRate30, double callbackRate7, double callbackRate30) CalculateForecasts(List<ServiceRequest> requests, DateTime now)
        {
            var last7 = requests.Where(r => r.CreatedAt >= now.AddDays(-7)).ToList();
            var last30 = requests.Where(r => r.CreatedAt >= now.AddDays(-30)).ToList();
            double closeRate7 = last7.Count > 0 ? last7.Count(r => r.Status == "Complete") / (double)last7.Count : 0;
            double closeRate30 = last30.Count > 0 ? last30.Count(r => r.Status == "Complete") / (double)last30.Count : 0;
            double callbackRate7 = last7.Count > 0 ? last7.Count(r => r.NeedsFollowUp) / (double)last7.Count : 0;
            double callbackRate30 = last30.Count > 0 ? last30.Count(r => r.NeedsFollowUp) / (double)last30.Count : 0;
            return (closeRate7, closeRate30, callbackRate7, callbackRate30);
        }

        public static (bool isAtRisk, List<string> riskFlags) EvaluateRisk(double closeRate7, double closeRate30, double callbackRate7, double callbackRate30, double closeRateThreshold = 0.8, double callbackRateThreshold = 0.1)
        {
            var riskFlags = new List<string>();
            if (closeRate7 < closeRateThreshold)
                riskFlags.Add($"7d close rate below {closeRateThreshold:P0}");
            if (closeRate30 < closeRateThreshold)
                riskFlags.Add($"30d close rate below {closeRateThreshold:P0}");
            if (callbackRate7 > callbackRateThreshold)
                riskFlags.Add($"7d callback rate above {callbackRateThreshold:P0}");
            if (callbackRate30 > callbackRateThreshold)
                riskFlags.Add($"30d callback rate above {callbackRateThreshold:P0}");
            return (riskFlags.Count > 0, riskFlags);
        }
        #endregion

        // Internal helper for testability
        public static (List<MonthlyKpiDto> closeRates, List<CallbackTrendDto> callbacks, double etaSuccess, double avgDelay) CalculateKpis(List<ServiceRequest> requests)
        {
            var closeRateTrends = requests
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new MonthlyKpiDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    CloseRate = g.Count(x => x.Status == "Complete") / (double)g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var callbackTrends = requests
                .Where(r => r.NeedsFollowUp)
                .GroupBy(r => r.CreatedAt.Date)
                .Select(g => new CallbackTrendDto
                {
                    Date = g.Key,
                    CallbackCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            int jobsWithEta = 0, jobsOnTime = 0;
            double totalDelayMinutes = 0;
            int delayedJobs = 0;
            foreach (var req in requests)
            {
                if (req.ScheduledAt.HasValue && req.ClosedAt.HasValue)
                {
                    jobsWithEta++;
                    var scheduled = req.ScheduledAt.Value;
                    var started = req.ClosedAt.Value;
                    if (started <= scheduled)
                    {
                        jobsOnTime++;
                    }
                    else
                    {
                        delayedJobs++;
                        totalDelayMinutes += (started - scheduled).TotalMinutes;
                    }
                }
            }
            double etaSuccessRate = jobsWithEta > 0 ? (jobsOnTime / (double)jobsWithEta) : 0;
            double avgDelay = delayedJobs > 0 ? (totalDelayMinutes / delayedJobs) : 0;
            return (closeRateTrends, callbackTrends, etaSuccessRate, avgDelay);
        }

        public async Task<List<TechnicianHeatmapCell>> GetHeatmapDataAsync(int techId, DateRange range)
        {
            var requests = await _db.ServiceRequests
                .Where(r => r.AssignedTechnicianId == techId && r.CreatedAt >= range.Start && r.CreatedAt <= range.End)
                .ToListAsync();

            // If no real data, return mock data for demo
            if (requests.Count == 0)
            {
                var mock = new List<TechnicianHeatmapCell>();
                var rand = new Random(techId);
                for (int d = 0; d < 7; d++)
                    for (int h = 0; h < 24; h++)
                        mock.Add(new TechnicianHeatmapCell
                        {
                            DayOfWeek = d,
                            HourOfDay = h,
                            Jobs = rand.Next(0, 3),
                            Delays = rand.Next(0, 2),
                            Callbacks = rand.Next(0, 2)
                        });
                return mock;
            }

            var grouped = requests
                .GroupBy(r => new { Day = (int)r.CreatedAt.DayOfWeek, Hour = r.CreatedAt.Hour })
                .Select(g => new TechnicianHeatmapCell
                {
                    DayOfWeek = g.Key.Day,
                    HourOfDay = g.Key.Hour,
                    Jobs = g.Count(),
                    Delays = g.Count(x => x.ClosedAt.HasValue && x.ScheduledAt.HasValue && x.ClosedAt > x.ScheduledAt),
                    Callbacks = g.Count(x => x.NeedsFollowUp)
                })
                .ToList();
            return grouped;
        }
    }
}
