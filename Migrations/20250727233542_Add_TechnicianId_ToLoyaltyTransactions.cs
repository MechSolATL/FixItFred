using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_TechnicianId_ToLoyaltyTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRewardSentAt",
                table: "TechnicianTrustLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "LoyaltyPointTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserOnboardingStatuses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsProsCertified = table.Column<bool>(type: "bit", nullable: false),
                    CertifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnboardingComplete = table.Column<bool>(type: "bit", nullable: false),
                    ChecklistStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOnboardingStatuses", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyPointTransactions_TechnicianId",
                table: "LoyaltyPointTransactions",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyPointTransactions_Technicians_TechnicianId",
                table: "LoyaltyPointTransactions",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyPointTransactions_Technicians_TechnicianId",
                table: "LoyaltyPointTransactions");

            migrationBuilder.DropTable(
                name: "UserOnboardingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LoyaltyPointTransactions_TechnicianId",
                table: "LoyaltyPointTransactions");

            migrationBuilder.DropColumn(
                name: "LastRewardSentAt",
                table: "TechnicianTrustLogs");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "LoyaltyPointTransactions");
        }
    }
}
