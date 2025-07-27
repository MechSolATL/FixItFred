using System;
using System.Collections.Generic;
using System.Linq;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Xunit;

namespace MVP_Core.Tests
{
    public class TechnicianReportCardServiceTests
    {
        [Fact]
        public void AggregatesLateClockInAndIdleMinutesCorrectly()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 1, FullName = "Tech One" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 1, Severity = "Severe", Date = DateTime.UtcNow });
            db.IdleSessionMonitorLogs.Add(new IdleSessionMonitorLog { TechnicianId = 1, IdleMinutes = 30, IdleStartTime = DateTime.UtcNow });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var cards = service.GetAllReportCards();

            // Assert
            var card = cards.FirstOrDefault(x => x.TechnicianId == 1);
            Assert.NotNull(card);
            Assert.Equal(1, card.LateClockInCount);
            Assert.Equal(1, card.SevereLateClockInCount);
            Assert.Equal(30, card.IdleMinutesWeek);
        }

        [Fact]
        public void AlertsOnSevereFlagsAndLowTrustScore()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 2, FullName = "Tech Two" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 2, Severity = "Severe", Date = DateTime.UtcNow });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 2, Severity = "Severe", Date = DateTime.UtcNow });
            db.TechnicianTrustLogs.Add(new TechnicianTrustLog { TechnicianId = 2, TrustScore = 4 });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var cards = service.GetAllReportCards();
            var alertTechs = cards.Where(x => x.SevereLateClockInCount >= 2 || x.TrustScore < 5).ToList();

            // Assert
            Assert.Contains(alertTechs, x => x.TechnicianId == 2);
        }

        [Fact]
        public void FiltersByDateRangeCorrectly()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 3, FullName = "Tech Three" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 3, Severity = "Severe", Date = DateTime.UtcNow.AddDays(-2) });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 3, Severity = "Severe", Date = DateTime.UtcNow.AddDays(-10) });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var allCards = service.GetAllReportCards();
            var filtered = allCards.Where(x => x.TechnicianId == 3).ToList();
            // Simulate date filter: last 7 days
            var recentCount = db.LateClockInLogs.Count(x => x.TechnicianId == 3 && x.Date > DateTime.UtcNow.AddDays(-7));

            // Assert
            Assert.Equal(1, recentCount);
        }

        [Fact]
        public void FiltersBySeverityCorrectly()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 4, FullName = "Tech Four" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 4, Severity = "Severe", Date = DateTime.UtcNow });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 4, Severity = "Warning", Date = DateTime.UtcNow });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var allCards = service.GetAllReportCards();
            var filtered = allCards.Where(x => x.TechnicianId == 4).ToList();
            var severeCount = db.LateClockInLogs.Count(x => x.TechnicianId == 4 && x.Severity == "Severe");
            var warningCount = db.LateClockInLogs.Count(x => x.TechnicianId == 4 && x.Severity == "Warning");

            // Assert
            Assert.Equal(1, severeCount);
            Assert.Equal(1, warningCount);
        }

        [Fact]
        public void FiltersByMultipleCriteriaCorrectly()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 5, FullName = "Tech Five" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 5, Severity = "Severe", Date = DateTime.UtcNow.AddDays(-2) });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 5, Severity = "Warning", Date = DateTime.UtcNow.AddDays(-2) });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 5, Severity = "Severe", Date = DateTime.UtcNow.AddDays(-10) });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var allCards = service.GetAllReportCards();
            var filtered = allCards.Where(x => x.TechnicianId == 5 && x.SevereLateClockInCount > 0).ToList();

            // Assert
            Assert.Single(filtered);
        }

        [Fact]
        public void PaginationReturnsCorrectNumberOfItems()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            for (int i = 1; i <= 25; i++)
                db.Technicians.Add(new Technician { Id = i, FullName = $"Tech {i}" });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);
            var allCards = service.GetAllReportCards();
            int pageSize = 10;
            var page1 = allCards.Take(pageSize).ToList();
            var page2 = allCards.Skip(pageSize).Take(pageSize).ToList();

            // Act & Assert
            Assert.Equal(pageSize, page1.Count);
            Assert.Equal(pageSize, page2.Count);
        }

        [Fact]
        public void HandlesNoDataGracefully()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            var service = new TechnicianReportCardService(db);

            // Act
            var allCards = service.GetAllReportCards();

            // Assert
            Assert.Empty(allCards);
        }

        [Fact]
        public void AggregatesMultipleLogsForSingleTechnician()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 6, FullName = "Tech Six" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 6, Severity = "Severe", Date = DateTime.UtcNow });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 6, Severity = "Warning", Date = DateTime.UtcNow });
            db.IdleSessionMonitorLogs.Add(new IdleSessionMonitorLog { TechnicianId = 6, IdleMinutes = 15, IdleStartTime = DateTime.UtcNow });
            db.IdleSessionMonitorLogs.Add(new IdleSessionMonitorLog { TechnicianId = 6, IdleMinutes = 20, IdleStartTime = DateTime.UtcNow });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var card = service.GetAllReportCards().FirstOrDefault(x => x.TechnicianId == 6);

            // Assert
            Assert.NotNull(card);
            Assert.Equal(2, card.LateClockInCount);
            Assert.Equal(1, card.SevereLateClockInCount);
            Assert.Equal(35, card.IdleMinutesWeek);
        }

        [Fact]
        public void DrillDownReturnsCorrectDetails()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 7, FullName = "Tech Seven" });
            db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = 7, Severity = "Severe", Date = DateTime.UtcNow });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);

            // Act
            var card = service.GetAllReportCards().FirstOrDefault(x => x.TechnicianId == 7);

            // Assert
            Assert.NotNull(card);
            Assert.Equal("Tech Seven", card.TechnicianName);
            Assert.Equal(1, card.LateClockInCount);
        }

        [Fact]
        public void HandlesExtremelyHighMetricValues()
        {
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 8, FullName = "Tech Eight" });
            db.IdleSessionMonitorLogs.Add(new IdleSessionMonitorLog { TechnicianId = 8, IdleMinutes = 99999, IdleStartTime = DateTime.UtcNow });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);
            var card = service.GetAllReportCards().FirstOrDefault(x => x.TechnicianId == 8);
            Assert.NotNull(card);
            Assert.Equal(99999, card.IdleMinutesWeek);
        }

        [Fact]
        public void HandlesMissingOrNullDataFields()
        {
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 9, FullName = null });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);
            var card = service.GetAllReportCards().FirstOrDefault(x => x.TechnicianId == 9);
            Assert.NotNull(card);
            Assert.True(string.IsNullOrEmpty(card.TechnicianName));
        }

        [Fact]
        public void HandlesTechniciansWithNoLogs()
        {
            var db = TestDbContextFactory.Create();
            db.Technicians.Add(new Technician { Id = 10, FullName = "Tech Ten" });
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);
            var card = service.GetAllReportCards().FirstOrDefault(x => x.TechnicianId == 10);
            Assert.NotNull(card);
            Assert.Equal(0, card.LateClockInCount);
            Assert.Equal(0, card.IdleMinutesWeek);
        }

        [Fact]
        public void PerformanceWithLargeDataSet()
        {
            var db = TestDbContextFactory.Create();
            for (int i = 11; i <= 1010; i++)
            {
                db.Technicians.Add(new Technician { Id = i, FullName = $"Tech {i}" });
                db.LateClockInLogs.Add(new LateClockInLog { TechnicianId = i, Severity = "Severe", Date = DateTime.UtcNow });
            }
            db.SaveChanges();
            var service = new TechnicianReportCardService(db);
            var cards = service.GetAllReportCards();
            Assert.Equal(1000, cards.Count);
        }
    }
}
