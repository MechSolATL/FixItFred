namespace MVP_Core.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            // ? Admin User Seeding
            if (!db.AdminUsers.Any())
            {
                _ = db.AdminUsers.Add(new AdminUser
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                    Role = "Admin"
                });
            }

            // ? Robots.txt Seeding
            if (!db.RobotsContents.Any())
            {
                _ = db.RobotsContents.Add(new RobotsContent
                {
                    Content = """
                    User-agent: *
                    Disallow: /Admin/
                    Disallow: /Account/
                    Disallow: /BlazorAdmin/
                    Disallow: /Identity/
                    Disallow: /Error
                    Disallow: /Verify
                    Disallow: /_Host
                    Disallow: /favicon.ico
                    Disallow: /backup/
                    Allow: /
                    Sitemap: https://service-atlanta.com/sitemap.xml
                    """,
                    UpdatedAt = DateTime.UtcNow
                });
            }

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
    }
}
