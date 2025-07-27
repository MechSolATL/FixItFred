using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddOvertimeDefenseLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeoBreakValidationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ValidationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    MinutesStationary = table.Column<int>(type: "int", nullable: false),
                    SystemDecision = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsOverride = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoBreakValidationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdleSessionMonitorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    IdleStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdleEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdleMinutes = table.Column<int>(type: "int", nullable: false),
                    SystemDecision = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsOverride = table.Column<bool>(type: "bit", nullable: false),
                    ClockOutTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdleSessionMonitorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeLockoutLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SystemDecision = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsOverride = table.Column<bool>(type: "bit", nullable: false),
                    ClockOutTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeLockoutLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoBreakValidationLogs");

            migrationBuilder.DropTable(
                name: "IdleSessionMonitorLogs");

            migrationBuilder.DropTable(
                name: "OvertimeLockoutLogs");
        }
    }
}
