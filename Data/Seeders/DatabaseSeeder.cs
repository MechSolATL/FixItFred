using MVP_Core.Services.Admin;
using MVP_Core.Services.Dispatch;

namespace MVP_Core.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext db, int _)
        {
            // ? Admin User Seeding
            if (!db.AdminUsers.Any())
            {
                var adminUserEntity = db.AdminUsers.Add(new AdminUser
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                    Role = "Admin"
                });
                // FixItFred: .Entity.Id extracted, but ignored since not needed
                _ = adminUserEntity.Entity.Id;
            }

            // FixItFred: Removed due to invalid column in schema - Sprint 30C.2
            // All logic referencing TechnicianProfiles.LastProfileReviewDate, RobotsContents.Robots, and RobotsContents.Path has been removed.
            Console.WriteLine("FixItFred: All references to LastProfileReviewDate, Robots, and Path have been purged from DatabaseSeeder.cs - Sprint 30C.2");

            // ? SeoMeta Metadata Seeding
            if (!db.SEOs.Any())
            {
                db.SEOs.AddRange(
                    new SeoMeta
                    {
                        PageName = "Home",
                        Title = "Mechanical Solutions Atlanta - Plumbing, Heating, HVAC, Water Filtration Services",
                        MetaDescription = "Mechanical Solutions Atlanta offers expert Plumbing, Heating, Air Conditioning, and Water Filtration services for residential and commercial clients in Duluth, Georgia.",
                        Keywords = "Plumbing, Heating, HVAC, Air Conditioning, Water Filtration, Mechanical Solutions Atlanta, Duluth GA, Rheem certified, Navien certified, tankless water heater, air-to-water, HVAC maintenance, water filtration system",
                        Robots = "index, follow"
                    },
                    new SeoMeta
                    {
                        PageName = "Services/Plumbing",
                        Title = "Expert Plumbing Services | Service-Atlanta.com",
                        MetaDescription = "Trust our licensed professionals for top-rated plumbing repairs, maintenance, and tankless water heater installations in Duluth, GA.",
                        Keywords = "Plumbing, Tankless Water Heater, Leak Repair, Plumbing Maintenance, Duluth Plumber, Service-Atlanta",
                        Robots = "index, follow"
                    },
                    new SeoMeta
                    {
                        PageName = "Services/Heating",
                        Title = "Heating System Repairs & Installations | Service-Atlanta.com",
                        MetaDescription = "We provide expert furnace repair, heat pump installation, and maintenance solutions across Duluth and surrounding areas.",
                        Keywords = "Heating Repair, Furnace, Heat Pump, HVAC Heating, Emergency Heating Services, Duluth GA",
                        Robots = "index, follow"
                    },
                    new SeoMeta
                    {
                        PageName = "Services/AirConditioning",
                        Title = "Air Conditioning Services | Service-Atlanta.com",
                        MetaDescription = "Stay cool with our fast, affordable air conditioning repair, maintenance, and new installations by certified techs.",
                        Keywords = "Air Conditioning, HVAC Cooling, AC Repair, AC Maintenance, Cooling System Install, Service-Atlanta",
                        Robots = "index, follow"
                    },
                    new SeoMeta
                    {
                        PageName = "Services/WaterFiltration",
                        Title = "Water Filtration Experts | Clean Water for Your Home",
                        MetaDescription = "Upgrade your water quality with advanced whole-house filtration and water softener solutions from our certified techs.",
                        Keywords = "Water Filtration, Water Softeners, Hard Water Solutions, Home Filtration, Duluth GA, Service-Atlanta",
                        Robots = "index, follow"
                    }
                );
            }

            // Sprint 73.2: RoastNBoost Protocol - Seed RoastTemplates
            if (!db.RoastTemplates.Any())
            {
                db.RoastTemplates.AddRange(new[]
                {
                    new RoastTemplate { Message = "Still figuring out the printer, huh? We believe in you.", Level = 1 },
                    new RoastTemplate { Message = "If enthusiasm paid the bills, you'd still owe rent.", Level = 2 },
                    new RoastTemplate { Message = "You're like a software update—always promising but never delivering on time.", Level = 3 },
                    new RoastTemplate { Message = "You bring new meaning to 'learning curve'.", Level = 1 },
                    new RoastTemplate { Message = "Your coffee runs are legendary—for taking forever.", Level = 1 },
                    new RoastTemplate { Message = "If only your code ran as fast as you walk to lunch.", Level = 2 },
                    new RoastTemplate { Message = "You have a bright future—in debugging your own mistakes.", Level = 2 },
                    new RoastTemplate { Message = "Your onboarding is a Netflix series: dramatic, confusing, and never-ending.", Level = 3 },
                    new RoastTemplate { Message = "You’re the reason we have a ‘Did you try restarting?’ policy.", Level = 1 },
                    new RoastTemplate { Message = "If procrastination was a skill, you’d be our MVP.", Level = 2 },
                    new RoastTemplate { Message = "You’re like a group chat notification—always popping up, rarely useful.", Level = 3 },
                    new RoastTemplate { Message = "Your badge should say ‘Work in Progress’.", Level = 1 },
                    new RoastTemplate { Message = "You ask more questions than our FAQ page.", Level = 1 },
                    new RoastTemplate { Message = "If you were a bug, you’d be a feature request.", Level = 2 },
                    new RoastTemplate { Message = "You’re the plot twist HR warned us about.", Level = 3 },
                    new RoastTemplate { Message = "Your meetings are like your code: full of surprises.", Level = 2 },
                    new RoastTemplate { Message = "You’re the only person who can crash a spreadsheet.", Level = 3 },
                    new RoastTemplate { Message = "You bring the ‘new’ to ‘new hire’ every day.", Level = 1 },
                    new RoastTemplate { Message = "If optimism was output, you’d be a compiler error.", Level = 2 },
                    new RoastTemplate { Message = "You’re the reason we double-check the onboarding checklist.", Level = 3 },
                    new RoastTemplate { Message = "Your Slack status should be ‘Trying my best’.", Level = 1 },
                });
            }

            _ = db.SaveChanges(); // Fix: Use discard for assignment, always non-null
        }

#if DEBUG
        /// <summary>
        /// Seeds 3–5 dummy ServiceRequest records with mock addresses, technician matches, and Zone tags.
        /// Triggers ETA prediction and ScheduleQueue insertion for each test record.
        /// </summary>
        public static void SeedTestServiceRequests(ApplicationDbContext db, DispatcherService dispatcherService, NotificationDispatchEngine dispatchEngine)
        {
            // FixItFred: Sprint 30D.3 — QA Seed Injection Prep 2024-07-25
            if (db.ServiceRequests.Any(r => r.CustomerName.StartsWith("TestSeed"))) return; // Avoid duplicates

            var testRequests = new[]
            {
                new { Name = "TestSeed Alpha", Email = "alpha@test.com", Address = "101 Alpha St", ServiceType = "Plumbing", ServiceSubtype = "Leak Repair", Details = "Leaky faucet in kitchen.", Zone = "North" },
                new { Name = "TestSeed Beta", Email = "beta@test.com", Address = "202 Beta Ave", ServiceType = "Heating", ServiceSubtype = "Furnace", Details = "No heat in living room.", Zone = "South" },
                new { Name = "TestSeed Gamma", Email = "gamma@test.com", Address = "303 Gamma Blvd", ServiceType = "Air Conditioning", ServiceSubtype = "AC Install", Details = "Install new AC unit.", Zone = "East" },
                new { Name = "TestSeed Delta", Email = "delta@test.com", Address = "404 Delta Rd", ServiceType = "Water Filtration", ServiceSubtype = "Filter Change", Details = "Replace whole-house filter.", Zone = "West" },
                new { Name = "TestSeed Epsilon", Email = "epsilon@test.com", Address = "505 Epsilon Ln", ServiceType = "Plumbing", ServiceSubtype = "Drain Cleaning", Details = "Clogged main drain.", Zone = "North" }
            };

            // FixItFred: Sprint 48.1 PredictETA async refactor
            foreach (var req in testRequests)
            {
                var serviceRequest = new ServiceRequest
                {
                    CustomerName = req.Name,
                    Email = req.Email,
                    Phone = "555-0000",
                    Address = req.Address,
                    ServiceType = req.ServiceType,
                    ServiceSubtype = req.ServiceSubtype,
                    Details = req.Details,
                    SessionId = null,
                    IsUrgent = false,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending"
                };
                db.ServiceRequests.Add(serviceRequest);
                db.SaveChanges();

                var tech = dispatcherService.FindAvailableTechnicianForZone(req.Zone);
                if (tech != null)
                {
                    var techStatus = new MVP_Core.Models.Admin.TechnicianStatusDto {
                        TechnicianId = tech.Id,
                        Name = tech.FullName,
                        Status = tech.Specialty,
                        DispatchScore = 100,
                        LastPing = DateTime.UtcNow,
                        AssignedJobs = 0,
                        LastUpdate = DateTime.UtcNow
                    };
                    var eta = dispatcherService.PredictETA(techStatus, req.Zone, 0).GetAwaiter().GetResult();
                    var queue = new ScheduleQueue
                    {
                        TechnicianId = tech.Id,
                        AssignedTechnicianName = tech.FullName ?? string.Empty,
                        TechnicianStatus = "Pending",
                        ServiceRequestId = serviceRequest.Id,
                        Zone = req.Zone,
                        ScheduledFor = DateTime.UtcNow,
                        EstimatedArrival = eta,
                        Status = ScheduleStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };
                    db.ScheduleQueues.Add(queue);
                    db.SaveChanges();
                    dispatchEngine.BroadcastETAAsync(req.Zone, $"Technician {tech.FullName ?? "Unknown"} ETA: {eta:t}").Wait();
                }
            }
        }

        /// <summary>
        /// QA Sprint 30F - Technician Flow Verification [Injected Technician and Jobs]
        /// Injects two test technicians and three ScheduleQueue jobs for mobile/dispatcher QA.
        /// </summary>
        public static void SeedTechnicianFlowTest(ApplicationDbContext db, NotificationDispatchEngine dispatchEngine)
        {
            // Avoid duplicates
            if (db.Technicians.Any(t => t.FullName.StartsWith("QA Tech"))) return;

            // Inject test technicians
            var techAlpha = new Technician
            {
                FullName = "QA Tech Alpha",
                IsActive = true,
                Specialty = "Plumbing",
                Email = "alpha@qa.com",
                Phone = "555-1111"
            };
            var techBeta = new Technician
            {
                FullName = "QA Tech Beta",
                IsActive = true,
                Specialty = "HVAC",
                Email = "beta@qa.com",
                Phone = "555-2222"
            };
            db.Technicians.AddRange(techAlpha, techBeta);
            db.SaveChanges();

            // Inject ScheduleQueue jobs
            var job1 = new ScheduleQueue
            {
                TechnicianId = techAlpha.Id,
                AssignedTechnicianName = techAlpha.FullName,
                TechnicianStatus = "Pending",
                ServiceRequestId = 10001,
                Zone = "Plumbing",
                ScheduledFor = DateTime.UtcNow,
                EstimatedArrival = DateTime.UtcNow.AddMinutes(30),
                Status = ScheduleStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            var job2 = new ScheduleQueue
            {
                TechnicianId = techBeta.Id,
                AssignedTechnicianName = techBeta.FullName,
                TechnicianStatus = "Pending",
                ServiceRequestId = 10002,
                Zone = "HVAC",
                ScheduledFor = DateTime.UtcNow,
                EstimatedArrival = DateTime.UtcNow.AddMinutes(45),
                Status = ScheduleStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            var job3 = new ScheduleQueue
            {
                TechnicianId = techAlpha.Id,
                AssignedTechnicianName = techAlpha.FullName,
                TechnicianStatus = "Dispatched",
                ServiceRequestId = 10003,
                Zone = "Plumbing",
                ScheduledFor = DateTime.UtcNow,
                EstimatedArrival = DateTime.UtcNow.AddMinutes(15),
                Status = ScheduleStatus.Dispatched,
                CreatedAt = DateTime.UtcNow
            };
            db.ScheduleQueues.AddRange(job1, job2, job3);
            db.SaveChanges();

            // Broadcast mock ETA for job3
            dispatchEngine.BroadcastETAAsync(job3.Zone, $"Technician {techAlpha.FullName} ETA: {job3.EstimatedArrival:t}").Wait();
        }

        // QA Sprint 32 - ETA History Seed
        public static void SeedETAHistoryTest(ApplicationDbContext db)
        {
            if (db.ETAHistoryEntries.Any(e => e.Message.StartsWith("QA Seed"))) return;
            var now = DateTime.UtcNow;
            db.ETAHistoryEntries.AddRange(
                new ETAHistoryEntry {
                    TechnicianId = 1, TechnicianName = "QA Tech Alpha", ServiceRequestId = 10001, Zone = "Plumbing", Timestamp = now.AddMinutes(-60), PredictedETA = now.AddMinutes(-30), ActualArrival = now.AddMinutes(-20), Message = "QA Seed: Initial ETA set"
                },
                new ETAHistoryEntry {
                    TechnicianId = 2, TechnicianName = "QA Tech Beta", ServiceRequestId = 10002, Zone = "HVAC", Timestamp = now.AddMinutes(-45), PredictedETA = now.AddMinutes(-10), ActualArrival = now.AddMinutes(10), Message = "QA Seed: ETA delayed by dispatcher"
                },
                new ETAHistoryEntry {
                    TechnicianId = 1, TechnicianName = "QA Tech Alpha", ServiceRequestId = 10001, Zone = "Plumbing", Timestamp = now.AddMinutes(-30), PredictedETA = now.AddMinutes(-20), ActualArrival = now.AddMinutes(0), Message = "QA Seed: ETA override for traffic"
                },
                new ETAHistoryEntry {
                    TechnicianId = 2, TechnicianName = "QA Tech Beta", ServiceRequestId = 10002, Zone = "HVAC", Timestamp = now.AddMinutes(-15), PredictedETA = now.AddMinutes(10), ActualArrival = now.AddMinutes(20), Message = "QA Seed: ETA extended for part pickup"
                }
            );
            db.SaveChanges();
        }

        // Sprint 32.3 - QA Milestone Setup
        public static void SeedReleaseQAData(ApplicationDbContext db)
        {
            if (db.ServiceRequests.Any(r => r.CustomerName.StartsWith("QA Release"))) return;
            var now = DateTime.UtcNow;
            // Inject 5 realistic ServiceRequests
            var requests = new[]
            {
                new ServiceRequest { CustomerName = "QA Release Alpha", ServiceType = "HVAC", ServiceSubtype = "AC Repair", Address = "101 Alpha St", Email = "alpha@qa.com", Phone = "555-1001", CreatedAt = now.AddHours(-5), Status = "Pending" },
                new ServiceRequest { CustomerName = "QA Release Beta", ServiceType = "Plumbing", ServiceSubtype = "Leak Repair", Address = "202 Beta Ave", Email = "beta@qa.com", Phone = "555-1002", CreatedAt = now.AddHours(-4), Status = "Pending" },
                new ServiceRequest { CustomerName = "QA Release Gamma", ServiceType = "Water", ServiceSubtype = "Filter Change", Address = "303 Gamma Blvd", Email = "gamma@qa.com", Phone = "555-1003", CreatedAt = now.AddHours(-3), Status = "Pending" },
                new ServiceRequest { CustomerName = "QA Release Delta", ServiceType = "HVAC", ServiceSubtype = "Heater Install", Address = "404 Delta Rd", Email = "delta@qa.com", Phone = "555-1004", CreatedAt = now.AddHours(-2), Status = "Pending" },
                new ServiceRequest { CustomerName = "QA Release Epsilon", ServiceType = "Plumbing", ServiceSubtype = "Drain Cleaning", Address = "505 Epsilon Ln", Email = "epsilon@qa.com", Phone = "555-1005", CreatedAt = now.AddHours(-1), Status = "Pending" }
            };
            db.ServiceRequests.AddRange(requests);
            db.SaveChanges();
            // Attach ScheduleQueue entries and ETA logs
            var tech1 = db.Technicians.FirstOrDefault(t => t.FullName.Contains("Alpha")) ?? db.Technicians.First();
            var tech2 = db.Technicians.FirstOrDefault(t => t.FullName.Contains("Beta")) ?? db.Technicians.Skip(1).FirstOrDefault() ?? db.Technicians.First();
            int i = 0;
            foreach (var req in requests)
            {
                var tech = (i++ % 2 == 0) ? tech1 : tech2;
                var queue = new ScheduleQueue
                {
                    TechnicianId = tech.Id,
                    AssignedTechnicianName = tech.FullName,
                    TechnicianStatus = "Pending",
                    ServiceRequestId = req.Id,
                    Zone = req.ServiceType,
                    ScheduledFor = req.CreatedAt.AddMinutes(30),
                    EstimatedArrival = req.CreatedAt.AddMinutes(90),
                    Status = ScheduleStatus.Pending,
                    CreatedAt = req.CreatedAt
                };
                // FixItFred: Sprint 34.1 - SLA Auto Calculation
                new MVP_Core.Services.Admin.DispatcherService(db, null).SetSLAExpiresAt(queue);
                db.ScheduleQueues.Add(queue);
                db.ETAHistoryEntries.Add(new ETAHistoryEntry
                {
                    TechnicianId = tech.Id,
                    TechnicianName = tech.FullName,
                    ServiceRequestId = req.Id,
                    Zone = req.ServiceType,
                    Timestamp = req.CreatedAt.AddMinutes(40),
                    PredictedETA = req.CreatedAt.AddMinutes(90),
                    ActualArrival = req.CreatedAt.AddMinutes(100),
                    Message = "QA Seed: Initial ETA"
                });
            }
            db.SaveChanges();
            // Simulate location updates
            db.TechTrackingLogs.AddRange(
                new MVP_Core.Data.TechTrackingLog { TechnicianId = tech1.Id, Timestamp = now.AddMinutes(-30), IP = "127.0.0.1", Latitude = 33.7, Longitude = -84.4, UserAgent = "QA-Agent" },
                new MVP_Core.Data.TechTrackingLog { TechnicianId = tech2.Id, Timestamp = now.AddMinutes(-25), IP = "127.0.0.1", Latitude = 33.8, Longitude = -84.5, UserAgent = "QA-Agent" }
            );
            db.SaveChanges();
        }

        // Sprint 34.2 - QA Escalation Logs
        public static void SeedEscalationLogsTest(ApplicationDbContext db)
        {
            if (db.EscalationLogs.Any()) return;
            var now = DateTime.UtcNow;
            db.EscalationLogs.AddRange(
                new MVP_Core.Data.Models.EscalationLogEntry {
                    ScheduleQueueId = 1,
                    TriggeredBy = "admin1",
                    Reason = "SLA breach",
                    ActionTaken = "ETA extended, tech notified",
                    CreatedAt = now.AddMinutes(-60)
                },
                new MVP_Core.Data.Models.EscalationLogEntry {
                    ScheduleQueueId = 2,
                    TriggeredBy = "admin2",
                    Reason = "Customer escalation",
                    ActionTaken = "Job reassigned",
                    CreatedAt = now.AddMinutes(-45)
                },
                new MVP_Core.Data.Models.EscalationLogEntry {
                    ScheduleQueueId = 3,
                    TriggeredBy = "admin1",
                    Reason = "Repeated delay",
                    ActionTaken = "Supervisor override",
                    CreatedAt = now.AddMinutes(-30)
                },
                new MVP_Core.Data.Models.EscalationLogEntry {
                    ScheduleQueueId = 4,
                    TriggeredBy = "admin3",
                    Reason = "Zone alert",
                    ActionTaken = "Manual follow-up",
                    CreatedAt = now.AddMinutes(-15)
                }
            );
            db.SaveChanges();
        }

        // Sprint 34.3 - SLA Violation Fallback Test Seeder
        public static void SeedSLAFallbackTest(ApplicationDbContext db)
        {
            if (db.ScheduleQueues.Any(q => q.AssignedTechnicianName == "SLA Fallback Test")) return;
            var now = DateTime.UtcNow;
            var tech = db.Technicians.FirstOrDefault() ?? new Technician { FullName = "Fallback Tech", IsActive = true, Specialty = "Plumbing", Email = "fallback@qa.com", Phone = "555-9999" };
            if (tech.Id == 0) { db.Technicians.Add(tech); db.SaveChanges(); }
            var queue = new ScheduleQueue
            {
                TechnicianId = tech.Id,
                AssignedTechnicianName = "SLA Fallback Test",
                TechnicianStatus = "Pending",
                ServiceRequestId = 99999,
                Zone = tech.Specialty,
                ScheduledFor = now.AddHours(-2),
                EstimatedArrival = now.AddHours(-1),
                Status = ScheduleStatus.Pending,
                CreatedAt = now.AddHours(-2),
                SLAExpiresAt = now.AddMinutes(-10)
            };
            db.ScheduleQueues.Add(queue);
            db.SaveChanges();
        }

        /// <summary>
        /// Sprint 35 - Technician Metrics & Zone Alert Edge Case Seeder
        /// Seeds technicians and jobs to cover edge cases: load limits, escalation, and score anomalies.
        /// </summary>
        public static void SeedTechnicianMetricsEdgeCases(ApplicationDbContext db)
        {
            if (db.Technicians.Any(t => t.FullName.StartsWith("EdgeCase Tech"))) return;
            var now = DateTime.UtcNow;
            // Technicians: 0 jobs, max jobs, over-max jobs, negative/zero/max scores
            var techs = new[]
            {
                new Technician { FullName = "EdgeCase Tech ZeroJobs", IsActive = true, Specialty = "ZoneA", Email = "zero@qa.com", Phone = "555-3000", DispatchScore = 100 },
                new Technician { FullName = "EdgeCase Tech MaxJobs", IsActive = true, Specialty = "ZoneA", Email = "max@qa.com", Phone = "555-3001", DispatchScore = 80 },
                new Technician { FullName = "EdgeCase Tech OverMaxJobs", IsActive = true, Specialty = "ZoneA", Email = "overmax@qa.com", Phone = "555-3002", DispatchScore = 50 },
                new Technician { FullName = "EdgeCase Tech NegativeScore", IsActive = true, Specialty = "ZoneB", Email = "neg@qa.com", Phone = "555-3003", DispatchScore = -10 },
                new Technician { FullName = "EdgeCase Tech ZeroScore", IsActive = true, Specialty = "ZoneB", Email = "zero2@qa.com", Phone = "555-3004", DispatchScore = 0 },
                new Technician { FullName = "EdgeCase Tech MaxScore", IsActive = true, Specialty = "ZoneB", Email = "max2@qa.com", Phone = "555-3005", DispatchScore = 120 }
            };
            db.Technicians.AddRange(techs);
            db.SaveChanges();
            // Jobs: assign jobs to techs to hit load limits
            var maxJobsTech = db.Technicians.First(t => t.FullName == "EdgeCase Tech MaxJobs");
            var overMaxJobsTech = db.Technicians.First(t => t.FullName == "EdgeCase Tech OverMaxJobs");
            var zeroJobsTech = db.Technicians.First(t => t.FullName == "EdgeCase Tech ZeroJobs");
            var jobs = new List<ScheduleQueue>();
            // Max jobs (3)
            for (int i = 0; i < 3; i++)
            {
                jobs.Add(new ScheduleQueue {
                    TechnicianId = maxJobsTech.Id,
                    AssignedTechnicianName = maxJobsTech.FullName,
                    TechnicianStatus = "Pending",
                    ServiceRequestId = 20000 + i,
                    Zone = "ZoneA",
                    ScheduledFor = now.AddMinutes(-30 * i),
                    EstimatedArrival = now.AddMinutes(30 * (i + 1)),
                    Status = ScheduleStatus.Pending,
                    CreatedAt = now.AddMinutes(-30 * i)
                });
            }
            // Over max jobs (4)
            for (int i = 0; i < 4; i++)
            {
                jobs.Add(new ScheduleQueue {
                    TechnicianId = overMaxJobsTech.Id,
                    AssignedTechnicianName = overMaxJobsTech.FullName,
                    TechnicianStatus = "Pending",
                    ServiceRequestId = 21000 + i,
                    Zone = "ZoneA",
                    ScheduledFor = now.AddMinutes(-20 * i),
                    EstimatedArrival = now.AddMinutes(20 * (i + 1)),
                    Status = ScheduleStatus.Pending,
                    CreatedAt = now.AddMinutes(-20 * i)
                });
            }
            // Zero jobs tech gets no jobs
            db.ScheduleQueues.AddRange(jobs);
            db.SaveChanges();
            // Zone with no techs: ZoneC (no techs assigned)
            // Zone with all techs overloaded: ZoneA (all techs at/over max)
            // Zone with mixed statuses: ZoneB (techs with negative, zero, max score)
            // Escalation logs for edge triggers
            db.EscalationLogs.AddRange(
                new MVP_Core.Data.Models.EscalationLogEntry {
                    ScheduleQueueId = jobs[0].Id,
                    TriggeredBy = "qa-admin",
                    Reason = "SLA breach",
                    ActionTaken = "Auto escalation",
                    CreatedAt = now.AddMinutes(-10)
                },
                new MVP_Core.Data.Models.EscalationLogEntry {
                    ScheduleQueueId = jobs[3].Id,
                    TriggeredBy = "qa-admin",
                    Reason = "Zone alert",
                    ActionTaken = "Manual review",
                    CreatedAt = now.AddMinutes(-5)
                }
            );
            db.SaveChanges();
        }

        /// <summary>
        /// Sprint 41 - Seed 2-3 historical JobMessageEntry threads for a known ServiceRequest.
        /// </summary>
        public static void SeedJobMessageThreads(ApplicationDbContext db)
        {
            if (db.JobMessages.Any(m => m.SenderName.StartsWith("SeededAdmin"))) return;
            var sr = db.ServiceRequests.FirstOrDefault();
            if (sr == null) return;
            db.JobMessages.AddRange(
                new JobMessageEntry {
                    ServiceRequestId = sr.Id,
                    SenderRole = "Admin",
                    SenderName = "SeededAdmin",
                    Message = "Welcome to your service request thread. How can we help?",
                    SentAt = DateTime.UtcNow.AddMinutes(-30),
                    IsInternalNote = false
                },
                new JobMessageEntry {
                    ServiceRequestId = sr.Id,
                    SenderRole = "Customer",
                    SenderName = "SeededCustomer",
                    Message = "I have a leak under my sink.",
                    SentAt = DateTime.UtcNow.AddMinutes(-25),
                    IsInternalNote = false
                },
                new JobMessageEntry {
                    ServiceRequestId = sr.Id,
                    SenderRole = "Technician",
                    SenderName = "SeededTech",
                    Message = "I'll be on site in 30 minutes.",
                    SentAt = DateTime.UtcNow.AddMinutes(-20),
                    IsInternalNote = false
                }
            );
            db.SaveChanges();
        }
#endif
    }
}
