namespace MVP_Core.Data.Seeders
{
    /// <summary>
    /// Seeds default application pages with access visibility and tracking metadata.
    /// </summary>
    public static class PagesSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (db.Pages.Any())
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            List<Models.Page> defaultPages =
            [
                new MVP_Core.Data.Models.Page { UrlPath = "/", IsPublic = true, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/plumbing", IsPublic = true, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/heating", IsPublic = true, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/air-conditioning", IsPublic = true, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/services/water-filtration", IsPublic = true, CreatedAt = now },

                // Admin-only pages
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/dashboard", IsPublic = false, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/backup-log", IsPublic = false, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/threats", IsPublic = false, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/server-logs", IsPublic = false, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/clicks", IsPublic = false, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/manage-SeoMeta", IsPublic = false, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/admin/manage-pages", IsPublic = false, CreatedAt = now },

                // Shared
                new MVP_Core.Data.Models.Page { UrlPath = "/shared/thankyou", IsPublic = true, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/shared/thankyousuccess", IsPublic = true, CreatedAt = now },
                new MVP_Core.Data.Models.Page { UrlPath = "/blocked", IsPublic = true, CreatedAt = now }
            ];

            db.Pages.AddRange(defaultPages);
            _ = db.SaveChanges();
        }
    }
}
