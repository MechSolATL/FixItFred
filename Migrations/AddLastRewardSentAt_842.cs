using Microsoft.EntityFrameworkCore.Migrations;
using System;

// Sprint 84.2 — Add LastRewardSentAt to TechnicianTrustLog
public partial class AddLastRewardSentAt_842 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "LastRewardSentAt",
            table: "TechnicianTrustLogs",
            type: "datetime2",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LastRewardSentAt",
            table: "TechnicianTrustLogs");
    }
}
