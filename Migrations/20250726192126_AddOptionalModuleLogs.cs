using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalModuleLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasRequiredMedia",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncComplianceCheckedAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminAlertLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAlertLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RootCauseCorrelationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootCauseCorrelationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SlaDriftSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRequests = table.Column<int>(type: "int", nullable: false),
                    DriftedRequests = table.Column<int>(type: "int", nullable: false),
                    AverageDriftMinutes = table.Column<double>(type: "float", nullable: false),
                    HeatmapJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaDriftSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageGrowthSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    GrowthBytes = table.Column<long>(type: "bigint", nullable: false),
                    CompressionRatio = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageGrowthSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemDiagnosticLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatusLevel = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDiagnosticLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianPerformanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianPerformanceLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminAlertLogs");

            migrationBuilder.DropTable(
                name: "RootCauseCorrelationLogs");

            migrationBuilder.DropTable(
                name: "SlaDriftSnapshots");

            migrationBuilder.DropTable(
                name: "StorageGrowthSnapshots");

            migrationBuilder.DropTable(
                name: "SystemDiagnosticLogs");

            migrationBuilder.DropTable(
                name: "TechnicianPerformanceLogs");

            migrationBuilder.DropColumn(
                name: "HasRequiredMedia",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "SyncComplianceCheckedAt",
                table: "ServiceRequests");
        }
    }
}
