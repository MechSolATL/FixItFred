using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncAutomationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoPromoted",
                table: "TechnicianSyncScores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BonusEligible",
                table: "TechnicianSyncScores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "CooldownRemaining",
                table: "TechnicianSyncScores",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CooldownUntil",
                table: "TechnicianSyncScores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBonusAwarded",
                table: "TechnicianSyncScores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecentPenaltyCount",
                table: "TechnicianSyncScores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StreakLength",
                table: "TechnicianSyncScores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoPromoted",
                table: "TechnicianSyncScores");

            migrationBuilder.DropColumn(
                name: "BonusEligible",
                table: "TechnicianSyncScores");

            migrationBuilder.DropColumn(
                name: "CooldownRemaining",
                table: "TechnicianSyncScores");

            migrationBuilder.DropColumn(
                name: "CooldownUntil",
                table: "TechnicianSyncScores");

            migrationBuilder.DropColumn(
                name: "LastBonusAwarded",
                table: "TechnicianSyncScores");

            migrationBuilder.DropColumn(
                name: "RecentPenaltyCount",
                table: "TechnicianSyncScores");

            migrationBuilder.DropColumn(
                name: "StreakLength",
                table: "TechnicianSyncScores");
        }
    }
}
