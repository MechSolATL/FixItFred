using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace MVP_Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SEO> SEOs { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SEO>().ToTable("SEOs");
            modelBuilder.Entity<Content>().ToTable("Content");
            modelBuilder.Entity<EmailVerification>().ToTable("EmailVerifications");
            modelBuilder.Entity<Question>().ToTable("Questions");
        }
    }
}
