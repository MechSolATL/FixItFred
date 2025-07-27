using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDispatcherFavoritismLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispatcherAssignmentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatcherId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceToJob = table.Column<double>(type: "float", nullable: false),
                    JobPriority = table.Column<int>(type: "int", nullable: false),
                    JobValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AutoAssigned = table.Column<bool>(type: "bit", nullable: false),
                    FlaggedForReview = table.Column<bool>(type: "bit", nullable: false),
                    JustificationJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatcherAssignmentLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoritismAlertLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatcherId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    PatternScore = table.Column<double>(type: "float", nullable: false),
                    FlaggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdminReviewed = table.Column<bool>(type: "bit", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritismAlertLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatcherAssignmentLogs");

            migrationBuilder.DropTable(
                name: "FavoritismAlertLogs");
        }
    }
}
