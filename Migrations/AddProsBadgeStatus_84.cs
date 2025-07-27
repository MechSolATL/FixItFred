// Sprint 84.0 — OnboardingStatus Schema
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVP_Core.Migrations
{
    public partial class AddProsBadgeStatus_84 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserOnboardingStatuses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    IsProsCertified = table.Column<bool>(nullable: false),
                    CertifiedOn = table.Column<DateTime>(nullable: true),
                    CertifiedBy = table.Column<string>(nullable: true),
                    OnboardingComplete = table.Column<bool>(nullable: false),
                    ChecklistStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOnboardingStatuses", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOnboardingStatuses");
        }
    }
}
