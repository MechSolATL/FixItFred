using Microsoft.EntityFrameworkCore;
using MVP_Core.Data.Models;
using MVP_Core.ViewModels.Admin;

namespace MVP_Core.Infrastructure.Context
{
    // Sprint93_04 — Stub added by FixItFred
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Sprint93_04 — Stub added by FixItFred
        public DbSet<TrustRebuildModel> TrustRebuilds { get; set; }
        public DbSet<WarningCenterModel> Warnings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sprint93_04 — Stub added by FixItFred
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TrustRebuildModel>()
                .HasKey(t => t.SystemId);
                
            modelBuilder.Entity<WarningCenterModel>()
                .HasKey(w => w.WarningId);
        }
    }
}