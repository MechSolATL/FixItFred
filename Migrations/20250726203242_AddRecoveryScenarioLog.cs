using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryScenarioLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecoveryScenarioLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenarioName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledForUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Executed = table.Column<bool>(type: "bit", nullable: false),
                    ExecutedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutcomeSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryScenarioLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecoveryScenarioLogs");
        }
    }
}
