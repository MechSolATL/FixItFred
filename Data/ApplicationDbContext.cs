using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data.Models;
using PageModel = MVP_Core.Data.Models.Page;

namespace MVP_Core.Data
{
    /// <summary>
    /// Unified database context for Service-Atlanta + Identity.
    /// </summary>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        #region DbSets for Identity + Custom Platform

        public DbSet<SeoMeta> SEOs { get; set; } = null!;
        public DbSet<Content> Contents { get; set; } = null!;
        public DbSet<EmailVerification> EmailVerifications { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<QuestionOption> QuestionOptions { get; set; } = null!;
        public DbSet<UserResponse> UserResponses { get; set; } = null!;
        public DbSet<ServiceRequest> ServiceRequests { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<AdminUser> AdminUsers { get; set; } = null!;
        public DbSet<PageModel> Pages { get; set; } = null!;
        public DbSet<BackgroundImage> BackgroundImages { get; set; } = null!;
        public DbSet<PageVisitLog> PageVisitLogs { get; set; } = null!;
        public DbSet<ThreatBlock> ThreatBlocks { get; set; } = null!;
        public DbSet<BackupLog> BackupLogs { get; set; } = null!;
        public DbSet<PageVisit> PageVisits { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<ProfileReview> ProfileReviews { get; set; } = null!;
        public DbSet<HeatPumpMatchup> HeatPumpMatchups { get; set; } = null!;
        public DbSet<EquipmentMatchup> EquipmentMatchups { get; set; } = null!;
        public DbSet<RobotsContent> RobotsContents { get; set; } = null!;
        public DbSet<BlacklistEntry> BlacklistEntries { get; set; } = null!;
        public DbSet<PendingEntry> PendingEntries { get; set; } = null!;
        public DbSet<BlacklistViewLog> BlacklistViewLogs { get; set; } = null!;
        public DbSet<ChallengeRequest> ChallengeRequests { get; set; } = null!;
        public DbSet<FlagLog> FlagLogs { get; set; } = null!;
        public DbSet<BotDetectionLog> BotDetectionLogs { get; set; } = null!;
        public DbSet<VettedApplication> VettedApplications { get; set; } = null!;
        public DbSet<LiveMetric> LiveMetrics { get; set; } = null!;
        public DbSet<QuickBooksIntegrationToken> QuickBooksIntegrationTokens { get; set; } = null!;
        public DbSet<BillingInvoiceRecord> BillingInvoiceRecords { get; set; } = null!;
        public DbSet<UserActivityLog> UserActivityLogs { get; set; } = null!;
        public DbSet<QBOAccountRecord> QBOAccounts { get; set; } = null!;
        public DbSet<TokenLog> TokenLogs { get; set; } = null!;
        public DbSet<FlaggedCustomer> FlaggedCustomers { get; set; } = null!;
        public DbSet<SearchLog> SearchLogs { get; set; } = null!;
        public DbSet<QBOAccountRecord> QBOAccountRecords { get; set; } = null!;
        public DbSet<Technician> Technicians { get; set; } = null!;
        public DbSet<KanbanHistoryLog> KanbanHistoryLogs { get; set; } = null!;
        public object? SeoMeta { get; set; }
        public DbSet<SlaSetting> SlaSettings { get; set; } = null!;
        public DbSet<TechnicianSkill> TechnicianSkills { get; set; } = null!;
        public DbSet<TechnicianSkillMap> TechnicianSkillMaps { get; set; } = null!;
        public DbSet<TechnicianLoadLog> TechnicianLoadLogs { get; set; } = null!;
        public DbSet<TechnicianMessage> TechnicianMessages { get; set; } = null!; // Sprint 21: Messaging model
        public DbSet<TechnicianFeedback> TechnicianFeedbacks { get; set; } = null!; // Sprint 22: Technician rating/feedback
        public DbSet<TechnicianMedia> TechnicianMedias { get; set; } = null!; // Sprint 23: Media uploads
        public DbSet<ScheduleQueue> ScheduleQueues { get; set; } = null!;
        public DbSet<ScheduleHistory> ScheduleHistories { get; set; } = null!;
        public DbSet<NotificationsSent> NotificationsSent { get; set; } = null!;
        public DbSet<ETAHistoryEntry> ETAHistoryEntries { get; set; } = null!;
        public DbSet<MVP_Core.Data.TechTrackingLog> TechTrackingLogs { get; set; } = null!; // Sprint 30E - Secure Technician GPS API
        // Sprint 32.2 - Security + Audit Harden
        public DbSet<MVP_Core.Data.Models.AuditLogEntry> AuditLogEntries { get; set; } = null!;
        // Sprint 34.2 - SLA Escalation Log Model
        public DbSet<EscalationLogEntry> EscalationLogs { get; set; } = null!;
        // Sprint 35 - Technician Reward Scoring System
        public DbSet<TechnicianScoreEntry> TechnicianScoreEntries { get; set; } = null!;
        public DbSet<TechnicianDeviceRegistration> TechnicianDeviceRegistrations { get; set; } = null!;

        #endregion

        #region Fluent Table Mappings

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.Entity<SeoMeta>().ToTable("SEOs");
            _ = modelBuilder.Entity<Content>().ToTable("Content");
            _ = modelBuilder.Entity<EmailVerification>().ToTable("EmailVerifications");
            _ = modelBuilder.Entity<Question>().ToTable("Questions");
            _ = modelBuilder.Entity<QuestionOption>().ToTable("QuestionOptions");
            _ = modelBuilder.Entity<UserResponse>().ToTable("UserResponses");
            _ = modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequests");
            _ = modelBuilder.Entity<Customer>().ToTable("Customers");
            _ = modelBuilder.Entity<AdminUser>().ToTable("AdminUsers");
            _ = modelBuilder.Entity<PageModel>().ToTable("Pages");
            _ = modelBuilder.Entity<BackgroundImage>().ToTable("BackgroundImages");
            _ = modelBuilder.Entity<PageVisitLog>().ToTable("PageVisitLogs");
            _ = modelBuilder.Entity<ThreatBlock>().ToTable("ThreatBlocks");
            _ = modelBuilder.Entity<BackupLog>().ToTable("BackupLogs");
            _ = modelBuilder.Entity<PageVisit>().ToTable("PageVisits");
            _ = modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            _ = modelBuilder.Entity<ProfileReview>().ToTable("ProfileReviews");
            _ = modelBuilder.Entity<HeatPumpMatchup>().ToTable("HeatPumpMatchups");
            _ = modelBuilder.Entity<EquipmentMatchup>().ToTable("EquipmentMatchups");
            _ = modelBuilder.Entity<RobotsContent>().ToTable("RobotsContents");
            _ = modelBuilder.Entity<TokenLog>().ToTable("TokenLogs");
            _ = modelBuilder.Entity<SlaSetting>().ToTable("SlaSettings");
            _ = modelBuilder.Entity<TechnicianSkill>().ToTable("TechnicianSkills");
            _ = modelBuilder.Entity<TechnicianSkillMap>().ToTable("TechnicianSkillMaps");
            _ = modelBuilder.Entity<TechnicianLoadLog>().ToTable("TechnicianLoadLogs");
            _ = modelBuilder.Entity<ScheduleQueue>().ToTable("ScheduleQueues");
            _ = modelBuilder.Entity<ScheduleHistory>().ToTable("ScheduleHistories");
            _ = modelBuilder.Entity<NotificationsSent>().ToTable("NotificationsSent");
            _ = modelBuilder.Entity<EscalationLogEntry>().ToTable("EscalationLogs");
            // Sprint 35 - Technician Reward Scoring System
            _ = modelBuilder.Entity<TechnicianScoreEntry>().ToTable("TechnicianScoreEntries");
            _ = modelBuilder.Entity<TechnicianDeviceRegistration>().ToTable("TechnicianDeviceRegistrations");
        }

        #endregion
    }
}
