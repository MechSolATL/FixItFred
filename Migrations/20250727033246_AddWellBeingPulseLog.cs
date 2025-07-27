using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddWellBeingPulseLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WellBeingPulseLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoodScore = table.Column<int>(type: "int", nullable: false),
                    StressLevel = table.Column<int>(type: "int", nullable: false),
                    WorkSatisfactionScore = table.Column<int>(type: "int", nullable: false),
                    OpenNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedsFollowUp = table.Column<bool>(type: "bit", nullable: false),
                    ManagerReviewed = table.Column<bool>(type: "bit", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellBeingPulseLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WellBeingPulseLogs");
        }
    }
}
