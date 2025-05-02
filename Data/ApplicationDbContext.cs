using Microsoft.EntityFrameworkCore;
using MVP_Core.Models;
using MVP_Core.Data.Models;

namespace MVP_Core.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        #region 📋 Database Tables (DbSets)

        public DbSet<SEO> SEOs { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<UserResponse> UserResponses { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<MVP_Core.Data.Models.Page> Pages { get; set; } // ✅ Fully qualified to avoid Razor Page conflict
        public DbSet<BackgroundImage> BackgroundImages { get; set; }
        public DbSet<PageVisitLog> PageVisitLogs { get; set; }
        public DbSet<ThreatBlock> ThreatBlocks { get; set; }
        public DbSet<BackupLog> BackupLogs { get; set; }
        public DbSet<PageVisit> PageVisits { get; set; } // ✅ Added missing PageVisits
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ProfileReview> ProfileReviews { get; set; } // ✅ Added for profile persistence
        public DbSet<HeatPumpMatchup> HeatPumpMatchups { get; set; }

        #endregion

        #region 🔧 Table Mappings

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SEO>().ToTable("SEOs");
            modelBuilder.Entity<Content>().ToTable("Content");
            modelBuilder.Entity<EmailVerification>().ToTable("EmailVerifications");
            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<QuestionOption>().ToTable("QuestionOptions");
            modelBuilder.Entity<UserResponse>().ToTable("UserResponses");
            modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequests");
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<AdminUser>().ToTable("AdminUsers");
            modelBuilder.Entity<MVP_Core.Data.Models.Page>().ToTable("Pages");
            modelBuilder.Entity<BackgroundImage>().ToTable("BackgroundImages");
            modelBuilder.Entity<PageVisitLog>().ToTable("PageVisitLogs");
            modelBuilder.Entity<ThreatBlock>().ToTable("ThreatBlocks");
            modelBuilder.Entity<BackupLog>().ToTable("BackupLogs");
            modelBuilder.Entity<PageVisit>().ToTable("PageVisits");
            modelBuilder.Entity<ProfileReview>().ToTable("ProfileReviews"); // ✅ Added table mapping
            modelBuilder.Entity<HeatPumpMatchup>().ToTable("HeatPumpMatchups");

        }

        #endregion
    }
}
