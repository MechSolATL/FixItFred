using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianForecastLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "TechnicianInsightLogs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "TechnicianForecastLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ForecastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectedJobs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpectedScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ForecastNotes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianForecastLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianForecastLogs");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "TechnicianInsightLogs");
        }
    }
}
