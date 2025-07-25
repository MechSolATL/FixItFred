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

            _ = db.SaveChanges();
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
                    var eta = dispatcherService.PredictETA(tech, req.Zone, 0);
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
#endif
    }
}
