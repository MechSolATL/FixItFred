using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class v205SanityOps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoastTemplateId",
                table: "NewHireRoastLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncidentCompressionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClusterKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurrenceCount = table.Column<int>(type: "int", nullable: false),
                    EquipmentFaults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchIssues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechBurnoutSuspected = table.Column<bool>(type: "bit", nullable: false),
                    ClientAbuseSuspected = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentCompressionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSanityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmotionalFatigueFlag = table.Column<bool>(type: "bit", nullable: false),
                    ErrorRateSpike = table.Column<int>(type: "int", nullable: false),
                    BurnoutPatternDetected = table.Column<bool>(type: "bit", nullable: false),
                    AIInterventionTriggered = table.Column<bool>(type: "bit", nullable: false),
                    FalseReportShielded = table.Column<bool>(type: "bit", nullable: false),
                    ShieldingNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSanityLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewHireRoastLogs_RoastTemplateId",
                table: "NewHireRoastLogs",
                column: "RoastTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewHireRoastLogs_RoastTemplates_RoastTemplateId",
                table: "NewHireRoastLogs",
                column: "RoastTemplateId",
                principalTable: "RoastTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewHireRoastLogs_RoastTemplates_RoastTemplateId",
                table: "NewHireRoastLogs");

            migrationBuilder.DropTable(
                name: "IncidentCompressionLogs");

            migrationBuilder.DropTable(
                name: "TechnicianSanityLogs");

            migrationBuilder.DropIndex(
                name: "IX_NewHireRoastLogs_RoastTemplateId",
                table: "NewHireRoastLogs");

            migrationBuilder.DropColumn(
                name: "RoastTemplateId",
                table: "NewHireRoastLogs");
        }
    }
}
