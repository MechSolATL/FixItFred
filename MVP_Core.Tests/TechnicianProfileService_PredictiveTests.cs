using Microsoft.EntityFrameworkCore;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using MVP_Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MVP_Core.Tests.Services
{
    public class TechnicianProfileService_PredictiveTests
    {
        [Fact]
        public void CalculateForecasts_LowCloseRate_FlagsRisk()
        {
            // Arrange: 10 jobs, only 6 complete in last 7 days
            var now = new DateTime(2025, 7, 24);
            var requests = new List<ServiceRequest>();
            for (int i = 0; i < 10; i++)
            {
                requests.Add(new ServiceRequest
                {
                    CreatedAt = now.AddDays(-i),
                    Status = i < 6 ? "Complete" : "Open",
                    NeedsFollowUp = i == 0 // only 1 callback
                });
            }
            // Act
            var (close7, close30, cb7, cb30) = TechnicianProfileService.CalculateForecasts(requests, now);
            var (isAtRisk, riskFlags) = TechnicianProfileService.EvaluateRisk(close7, close30, cb7, cb30);
            // Assert
            Assert.True(isAtRisk);
            Assert.Contains(riskFlags, x => x.Contains("close rate"));
        }

        [Fact]
        public void EvaluateRisk_HighCallbackRate_FlagsRisk()
        {
            // Arrange: 10 jobs, 2 callbacks in last 7 days
            var now = new DateTime(2025, 7, 24);
            var requests = new List<ServiceRequest>();
            for (int i = 0; i < 10; i++)
            {
                requests.Add(new ServiceRequest
                {
                    CreatedAt = now.AddDays(-i),
                    Status = "Complete",
                    NeedsFollowUp = i < 2
                });
            }
            // Act
            var (close7, close30, cb7, cb30) = TechnicianProfileService.CalculateForecasts(requests, now);
            var (isAtRisk, riskFlags) = TechnicianProfileService.EvaluateRisk(close7, close30, cb7, cb30, 0.8, 0.1);
            // Assert
            Assert.True(isAtRisk);
            Assert.Contains(riskFlags, x => x.Contains("callback rate"));
        }

        [Fact]
        public async Task GetHeatmapDataAsync_AggregatesCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            using var db = new ApplicationDbContext(options);
            int techId = 1;
            var now = new DateTime(2025, 7, 24, 10, 0, 0);
            db.ServiceRequests.AddRange(new[] {
                new ServiceRequest { AssignedTechnicianId = techId, CreatedAt = now, Status = "Complete", NeedsFollowUp = false, ScheduledAt = now.AddMinutes(-30), ClosedAt = now },
                new ServiceRequest { AssignedTechnicianId = techId, CreatedAt = now, Status = "Complete", NeedsFollowUp = true, ScheduledAt = now.AddMinutes(-60), ClosedAt = now.AddMinutes(10) }, // delayed, callback
                new ServiceRequest { AssignedTechnicianId = techId, CreatedAt = now.AddHours(1), Status = "Open", NeedsFollowUp = false },
            });
            db.SaveChanges();
            var svc = new TechnicianProfileService(db);
            var range = new DateRange { Start = now.Date, End = now.Date.AddDays(1) };
            // Act
            var result = await svc.GetHeatmapDataAsync(techId, range);
            // Assert
            Assert.NotNull(result);
            var cell = result.FirstOrDefault(x => x.DayOfWeek == (int)now.DayOfWeek && x.HourOfDay == now.Hour);
            Assert.NotNull(cell);
            Assert.Equal(2, cell.Jobs); // 2 jobs at 10am
            Assert.Equal(1, cell.Delays); // 1 delayed
            Assert.Equal(1, cell.Callbacks); // 1 callback
        }
    }
}
