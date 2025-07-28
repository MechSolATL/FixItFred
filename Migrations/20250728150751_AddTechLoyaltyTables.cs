using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTechLoyaltyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertType",
                table: "TechnicianAlertLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TechnicianAlertLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "TechnicianAlertLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceRequestId",
                table: "TechnicianAlertLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalTime",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceCompletedAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MilestoneAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    MilestoneId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilestoneAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechMilestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechMilestones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    MilestoneId = table.Column<int>(type: "int", nullable: false),
                    ProgressCount = table.Column<int>(type: "int", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "bit", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechProgresses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MilestoneAuditLogs");

            migrationBuilder.DropTable(
                name: "TechMilestones");

            migrationBuilder.DropTable(
                name: "TechProgresses");

            migrationBuilder.DropColumn(
                name: "AlertType",
                table: "TechnicianAlertLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TechnicianAlertLogs");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "TechnicianAlertLogs");

            migrationBuilder.DropColumn(
                name: "ServiceRequestId",
                table: "TechnicianAlertLogs");

            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "InvoiceCompletedAt",
                table: "ServiceRequests");
        }
    }
}
