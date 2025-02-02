using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // DbSets
        public DbSet<SEO> SEOs { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; } // Add ServiceRequest table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table Configurations
            modelBuilder.Entity<SEO>().ToTable("SEOs");
            modelBuilder.Entity<Content>().ToTable("Content");
            modelBuilder.Entity<EmailVerification>().ToTable("EmailVerifications");
            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequests"); // Configure ServiceRequest table

            // Optional: Configure additional properties or relationships
        }
    }
}
