using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class Sprint61_TechGPSRoutePlayback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KanbanHistoryLogs_ServiceRequests_ServiceRequestId",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropIndex(
                name: "IX_KanbanHistoryLogs_ServiceRequestId",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobMessageEntries",
                table: "JobMessageEntries");

            migrationBuilder.DropColumn(
                name: "MessageBody",
                table: "TechnicianMessages");

            migrationBuilder.DropColumn(
                name: "SenderType",
                table: "TechnicianMessages");

            migrationBuilder.DropColumn(
                name: "FromIndex",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "JobMessageEntries");

            migrationBuilder.DropColumn(
                name: "ReadBy",
                table: "JobMessageEntries");

            migrationBuilder.DropColumn(
                name: "ReadTimestamp",
                table: "JobMessageEntries");

            migrationBuilder.RenameTable(
                name: "JobMessageEntries",
                newName: "JobMessages");

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Technicians",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSecondChance",
                table: "Technicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingStatus",
                table: "Technicians",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresSupervision",
                table: "Technicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "TechnicianMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TechnicianMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencyPledge",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyPledgeNotes",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrival",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalizedPDFPath",
                table: "ServiceRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinancingAPR",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinancingAmount",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinancingMonthlyPayment",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancingTermMonths",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FraudLogNotes",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFraudSuspected",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RoutePlaybackPath",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInTimeline",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TechnicianCommission",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "ScheduleQueues",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DispatcherOverride",
                table: "ScheduleQueues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedDurationHours",
                table: "ScheduleQueues",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeoDistanceKm",
                table: "ScheduleQueues",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeoDistanceToJob",
                table: "ScheduleQueues",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmergency",
                table: "ScheduleQueues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTechnicianAvailable",
                table: "ScheduleQueues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "ScheduleQueues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OptimizedETA",
                table: "ScheduleQueues",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverrideReason",
                table: "ScheduleQueues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredTechnicianId",
                table: "ScheduleQueues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RouteScore",
                table: "ScheduleQueues",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SLAWindowEnd",
                table: "ScheduleQueues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SLAWindowStart",
                table: "ScheduleQueues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceTypePriority",
                table: "ScheduleQueues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ToStatus",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ToIndex",
                table: "KanbanHistoryLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FromStatus",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ChangedBy",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerformedBy",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PerformedByRole",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "KanbanHistoryLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcknowledgedBy",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcknowledgedDate",
                table: "BillingInvoiceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountDue",
                table: "BillingInvoiceRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "BillingInvoiceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerSignaturePath",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "BillingInvoiceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "BillingInvoiceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "BillingInvoiceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalInvoiceId",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "BillingInvoiceRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAcknowledged",
                table: "BillingInvoiceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RealmId",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RetrievedAtUtc",
                table: "BillingInvoiceRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SignatureAuditLog",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WasAccepted",
                table: "BillingInvoiceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasExportedForLegalReview",
                table: "BillingInvoiceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "SenderRole",
                table: "JobMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "JobMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "JobMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobMessages",
                table: "JobMessages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CertificationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificationUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerBonusLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    RewardType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateEarned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateClaimed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsClaimed = table.Column<bool>(type: "bit", nullable: false),
                    SourceTrigger = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoyaltyPointsAwarded = table.Column<int>(type: "int", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClaimCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBonusLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsGamifiedBonus = table.Column<bool>(type: "bit", nullable: false),
                    BadgeAwarded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentimentScore = table.Column<float>(type: "real", nullable: true),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisputeRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EscalationLevel = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackSurveyTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InputType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackSurveyTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FollowUpActionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RelatedServiceRequestId = table.Column<int>(type: "int", nullable: true),
                    RelatedRewardId = table.Column<int>(type: "int", nullable: true),
                    EscalationLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenCount = table.Column<int>(type: "int", nullable: true),
                    ClickCount = table.Column<int>(type: "int", nullable: true),
                    ConversionCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpActionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyPointTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedReviewId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyPointTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationQueues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recipient = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChannelType = table.Column<int>(type: "int", nullable: false),
                    TriggerType = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MessageBody = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Zone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RewardType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    MinJobsRequired = table.Column<int>(type: "int", nullable: false),
                    ReferralBonus = table.Column<int>(type: "int", nullable: false),
                    StreakRequired = table.Column<int>(type: "int", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoyaltyPointsAwarded = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerCustomerId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    FraudFlagLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferralEventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralCodeId = table.Column<int>(type: "int", nullable: false),
                    ReferrerCustomerId = table.Column<int>(type: "int", nullable: true),
                    ReferredCustomerId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralEventLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PointsRequired = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BonusPoints = table.Column<int>(type: "int", nullable: false),
                    BadgeIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecondChanceFlagLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TriggeredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOverrideApproved = table.Column<bool>(type: "bit", nullable: false),
                    OverrideNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondChanceFlagLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillBadges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillBadges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    SkillTrackId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillProgresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillTracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteTag = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianDeviceRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianDeviceRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianPayRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursWorked = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BonusNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianPayRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyTemplateId = table.Column<int>(type: "int", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResponseValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackResponses_FeedbackSurveyTemplates_SurveyTemplateId",
                        column: x => x.SurveyTemplateId,
                        principalTable: "FeedbackSurveyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackResponses_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackResponses_ServiceRequestId",
                table: "FeedbackResponses",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackResponses_SurveyTemplateId",
                table: "FeedbackResponses",
                column: "SurveyTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificationRecords");

            migrationBuilder.DropTable(
                name: "CertificationUploads");

            migrationBuilder.DropTable(
                name: "CustomerBonusLogs");

            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "DisputeRecords");

            migrationBuilder.DropTable(
                name: "FeedbackResponses");

            migrationBuilder.DropTable(
                name: "FollowUpActionLogs");

            migrationBuilder.DropTable(
                name: "LoyaltyPointTransactions");

            migrationBuilder.DropTable(
                name: "NotificationQueues");

            migrationBuilder.DropTable(
                name: "PromotionEvents");

            migrationBuilder.DropTable(
                name: "ReferralCodes");

            migrationBuilder.DropTable(
                name: "ReferralEventLogs");

            migrationBuilder.DropTable(
                name: "RewardTiers");

            migrationBuilder.DropTable(
                name: "SecondChanceFlagLogs");

            migrationBuilder.DropTable(
                name: "SkillBadges");

            migrationBuilder.DropTable(
                name: "SkillProgresses");

            migrationBuilder.DropTable(
                name: "SkillTracks");

            migrationBuilder.DropTable(
                name: "TechnicianAuditLogs");

            migrationBuilder.DropTable(
                name: "TechnicianDeviceRegistrations");

            migrationBuilder.DropTable(
                name: "TechnicianPayRecords");

            migrationBuilder.DropTable(
                name: "FeedbackSurveyTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobMessages",
                table: "JobMessages");

            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "IsSecondChance",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "OnboardingStatus",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "RequiresSupervision",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "TechnicianMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TechnicianMessages");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "EmergencyPledge",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "EmergencyPledgeNotes",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "EstimatedArrival",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "FinalizedPDFPath",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "FinancingAPR",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "FinancingAmount",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "FinancingMonthlyPayment",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "FinancingTermMonths",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "FraudLogNotes",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsFraudSuspected",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RoutePlaybackPath",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ShowInTimeline",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "TechnicianCommission",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "DispatcherOverride",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "EstimatedDurationHours",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "GeoDistanceKm",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "GeoDistanceToJob",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "IsEmergency",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "IsTechnicianAvailable",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "OptimizedETA",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "OverrideReason",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "PreferredTechnicianId",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "RouteScore",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "SLAWindowEnd",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "SLAWindowStart",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "ServiceTypePriority",
                table: "ScheduleQueues");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropColumn(
                name: "PerformedBy",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropColumn(
                name: "PerformedByRole",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "KanbanHistoryLogs");

            migrationBuilder.DropColumn(
                name: "AcknowledgedBy",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "AcknowledgedDate",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "AmountDue",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "CustomerSignaturePath",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "ExternalInvoiceId",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "IsAcknowledged",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "RealmId",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "RetrievedAtUtc",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "SignatureAuditLog",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "WasAccepted",
                table: "BillingInvoiceRecords");

            migrationBuilder.DropColumn(
                name: "WasExportedForLegalReview",
                table: "BillingInvoiceRecords");

            migrationBuilder.RenameTable(
                name: "JobMessages",
                newName: "JobMessageEntries");

            migrationBuilder.AddColumn<string>(
                name: "MessageBody",
                table: "TechnicianMessages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderType",
                table: "TechnicianMessages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ToStatus",
                table: "KanbanHistoryLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ToIndex",
                table: "KanbanHistoryLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FromStatus",
                table: "KanbanHistoryLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ChangedBy",
                table: "KanbanHistoryLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FromIndex",
                table: "KanbanHistoryLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "BillingInvoiceRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SenderRole",
                table: "JobMessageEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "JobMessageEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "JobMessageEntries",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "JobMessageEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReadBy",
                table: "JobMessageEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadTimestamp",
                table: "JobMessageEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobMessageEntries",
                table: "JobMessageEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanHistoryLogs_ServiceRequestId",
                table: "KanbanHistoryLogs",
                column: "ServiceRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_KanbanHistoryLogs_ServiceRequests_ServiceRequestId",
                table: "KanbanHistoryLogs",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
