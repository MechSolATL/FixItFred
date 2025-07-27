using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddHeatmapAndRoutingOverlays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoutingOverlayRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeoBoundaryJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredTechIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZonePriority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingOverlayRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianHeatmapLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZoneTag = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianHeatmapLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianLoyaltyLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Milestone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianLoyaltyLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianScheduleConflictLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    ConflictDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConflictType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolutionSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManualOverrideNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianScheduleConflictLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutingOverlayRegions");

            migrationBuilder.DropTable(
                name: "TechnicianHeatmapLogs");

            migrationBuilder.DropTable(
                name: "TechnicianLoyaltyLogs");

            migrationBuilder.DropTable(
                name: "TechnicianScheduleConflictLogs");
        }
    }
}
