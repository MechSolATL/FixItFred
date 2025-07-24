namespace MVP_Core.Data.Contexts
{
    public class SeoDbContext : DbContext
    {
        public SeoDbContext(DbContextOptions<SeoDbContext> options) : base(options) { }

        public DbSet<SeoMeta> SeoMetaTags { get; set; } = null!;
    }
}
