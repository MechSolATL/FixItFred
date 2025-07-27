using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRoastRouletteFieldsToEmployeeMilestoneLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "RoastTemplates",
                newName: "UseLimit");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "RoastTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tier",
                table: "RoastTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimesUsed",
                table: "RoastTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptedOutOfRoasts",
                table: "EmployeeMilestoneLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRoastDeliveredAt",
                table: "EmployeeMilestoneLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoastTierPreference",
                table: "EmployeeMilestoneLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "TimesUsed",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "IsOptedOutOfRoasts",
                table: "EmployeeMilestoneLogs");

            migrationBuilder.DropColumn(
                name: "LastRoastDeliveredAt",
                table: "EmployeeMilestoneLogs");

            migrationBuilder.DropColumn(
                name: "RoastTierPreference",
                table: "EmployeeMilestoneLogs");

            migrationBuilder.RenameColumn(
                name: "UseLimit",
                table: "RoastTemplates",
                newName: "Level");
        }
    }
}
