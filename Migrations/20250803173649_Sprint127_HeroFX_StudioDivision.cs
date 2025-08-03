using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class Sprint127_HeroFX_StudioDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SEOs");

            migrationBuilder.AddColumn<bool>(
                name: "EnableBanterMode",
                table: "Technicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Technicians",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NicknameApproved",
                table: "Technicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "PatchReputationScore",
                table: "Technicians",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SEOs",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Robots",
                table: "SEOs",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "SEOs",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "SEOs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "RecoveryLearningLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ErrorObject",
                table: "RecoveryLearningLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRecorded",
                table: "RecoveryLearningLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ServiceRequest",
                table: "RecoveryLearningLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BanterReplayEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TriggerSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanterReplayEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpathyPrompts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpathyPrompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroImpactEffects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CssClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnimationCss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsFunction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    TriggerEvents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonaAssignments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleAssignments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BehaviorMoods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoiceFxConfig = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoiceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoundEffectPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsMobileCompatible = table.Column<bool>(type: "bit", nullable: false),
                    IsDesktopCompatible = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    FxPackCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    ReactionCount = table.Column<int>(type: "int", nullable: false),
                    KapowToClickRatio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroImpactEffects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnownFixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommonFix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuccessCount = table.Column<int>(type: "int", nullable: false),
                    LastConfirmed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownFixes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LLMModelProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LLMModelProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatchSystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatchSystemLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptTraceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptTraceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PromptText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewResponseLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false),
                    ResponseSuccess = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewResponseLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevitalizeTenants",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevitalizeTenants", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianReportFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    FeedbackNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianReportFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AIAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TroubleshootingAttemptLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromptInput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuggestedFix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    TechNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TroubleshootingAttemptLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroFxAnalyticsLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffectId = table.Column<int>(type: "int", nullable: false),
                    EffectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TriggerEvent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    GotReaction = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroFxAnalyticsLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroFxAnalyticsLogs_HeroImpactEffects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "HeroImpactEffects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromptExperiments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromptVersionId = table.Column<int>(type: "int", nullable: false),
                    ExperimentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptExperiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptExperiments_PromptVersions_PromptVersionId",
                        column: x => x.PromptVersionId,
                        principalTable: "PromptVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevitalizeTechnicianProfiles",
                columns: table => new
                {
                    TechnicianId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TrustScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedJobs = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Specializations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevitalizeTechnicianProfiles", x => x.TechnicianId);
                    table.ForeignKey(
                        name: "FK_RevitalizeTechnicianProfiles_RevitalizeTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "RevitalizeTenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevitalizeServiceRequests",
                columns: table => new
                {
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedTechnicianId = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevitalizeServiceRequests", x => x.ServiceRequestId);
                    table.ForeignKey(
                        name: "FK_RevitalizeServiceRequests_RevitalizeTechnicianProfiles_AssignedTechnicianId",
                        column: x => x.AssignedTechnicianId,
                        principalTable: "RevitalizeTechnicianProfiles",
                        principalColumn: "TechnicianId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RevitalizeServiceRequests_RevitalizeTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "RevitalizeTenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroFxAnalyticsLogs_EffectId",
                table: "HeroFxAnalyticsLogs",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptExperiments_PromptVersionId",
                table: "PromptExperiments",
                column: "PromptVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_RevitalizeServiceRequests_AssignedTechnicianId",
                table: "RevitalizeServiceRequests",
                column: "AssignedTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_RevitalizeServiceRequests_TenantId",
                table: "RevitalizeServiceRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RevitalizeTechnicianProfiles_TenantId",
                table: "RevitalizeTechnicianProfiles",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BanterReplayEvents");

            migrationBuilder.DropTable(
                name: "EmpathyPrompts");

            migrationBuilder.DropTable(
                name: "HeroFxAnalyticsLogs");

            migrationBuilder.DropTable(
                name: "KnownFixes");

            migrationBuilder.DropTable(
                name: "LLMModelProviders");

            migrationBuilder.DropTable(
                name: "PatchSystemLogs");

            migrationBuilder.DropTable(
                name: "PromptExperiments");

            migrationBuilder.DropTable(
                name: "PromptTraceLogs");

            migrationBuilder.DropTable(
                name: "ReviewResponseLogs");

            migrationBuilder.DropTable(
                name: "RevitalizeServiceRequests");

            migrationBuilder.DropTable(
                name: "TechnicianReportFeedbacks");

            migrationBuilder.DropTable(
                name: "TechnicianReports");

            migrationBuilder.DropTable(
                name: "TroubleshootingAttemptLogs");

            migrationBuilder.DropTable(
                name: "HeroImpactEffects");

            migrationBuilder.DropTable(
                name: "PromptVersions");

            migrationBuilder.DropTable(
                name: "RevitalizeTechnicianProfiles");

            migrationBuilder.DropTable(
                name: "RevitalizeTenants");

            migrationBuilder.DropColumn(
                name: "EnableBanterMode",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "NicknameApproved",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "PatchReputationScore",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "RecoveryLearningLogs");

            migrationBuilder.DropColumn(
                name: "ErrorObject",
                table: "RecoveryLearningLogs");

            migrationBuilder.DropColumn(
                name: "LastRecorded",
                table: "RecoveryLearningLogs");

            migrationBuilder.DropColumn(
                name: "ServiceRequest",
                table: "RecoveryLearningLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SEOs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Robots",
                table: "SEOs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "SEOs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Keywords",
                table: "SEOs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SEOs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
