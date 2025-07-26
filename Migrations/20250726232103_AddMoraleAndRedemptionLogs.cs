using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMoraleAndRedemptionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicianMoraleLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    MoraleScore = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrustImpact = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianMoraleLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianRedemptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    PreviousTrustScore = table.Column<int>(type: "int", nullable: false),
                    RestoredTrustScore = table.Column<int>(type: "int", nullable: false),
                    ActionsTaken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianRedemptionLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianMoraleLogs");

            migrationBuilder.DropTable(
                name: "TechnicianRedemptionLogs");
        }
    }
}
