using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryTraceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceRequestId",
                table: "RecoveryScenarioLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinkedRequestId",
                table: "RecoveryLearningLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceModule",
                table: "RecoveryLearningLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TriggerContextJson",
                table: "RecoveryLearningLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryScenarioLogs_ServiceRequestId",
                table: "RecoveryScenarioLogs",
                column: "ServiceRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecoveryScenarioLogs_ServiceRequests_ServiceRequestId",
                table: "RecoveryScenarioLogs",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecoveryScenarioLogs_ServiceRequests_ServiceRequestId",
                table: "RecoveryScenarioLogs");

            migrationBuilder.DropIndex(
                name: "IX_RecoveryScenarioLogs_ServiceRequestId",
                table: "RecoveryScenarioLogs");

            migrationBuilder.DropColumn(
                name: "ServiceRequestId",
                table: "RecoveryScenarioLogs");

            migrationBuilder.DropColumn(
                name: "LinkedRequestId",
                table: "RecoveryLearningLogs");

            migrationBuilder.DropColumn(
                name: "SourceModule",
                table: "RecoveryLearningLogs");

            migrationBuilder.DropColumn(
                name: "TriggerContextJson",
                table: "RecoveryLearningLogs");
        }
    }
}
