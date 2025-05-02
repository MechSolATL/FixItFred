using MVP_Core.Data.Models;

namespace MVP_Core.Data.Seeders
{
    public static class PagesSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (db.Pages.Any())
                return;

            var defaultPages = new List<MVP_Core.Data.Models.Page>
            {
                new MVP_Core.Data.Models.Page { UrlPath = "/", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/plumbing", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/heating", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/air-conditioning", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/water-filtration", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/dashboard", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/backup-log", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/threats", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/server-logs", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/clicks", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/manage-seo", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/manage-pages", IsPublic = false, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/shared/thankyou", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/shared/thankyousuccess", IsPublic = true, CreatedAt = DateTime.UtcNow },
                new MVP_Core.Data.Models.Page { UrlPath = "/blocked", IsPublic = true, CreatedAt = DateTime.UtcNow },
            };

            db.Pages.AddRange(defaultPages); // ✅ Corrected: add entire list
            db.SaveChanges();
        }
    }
}
