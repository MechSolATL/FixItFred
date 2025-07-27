using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchema_207Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "EmployeeOnboardingProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "EmployeeOnboardingProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "EmployeeOnboardingProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FilePaths",
                table: "EmployeeOnboardingProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "EmployeeOnboardingProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "EmployeeOnboardingProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EmployeeOnboardingProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RoastDeliveryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoastTemplateId = table.Column<int>(type: "int", nullable: false),
                    TriggeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryResult = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoastDeliveryLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoastDeliveryLogs");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "EmployeeOnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "EmployeeOnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "EmployeeOnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "FilePaths",
                table: "EmployeeOnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "EmployeeOnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "EmployeeOnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmployeeOnboardingProfiles");
        }
    }
}

// Sprint 83.7-Hardening: Resolved schema mismatch (SQL Error 207)
