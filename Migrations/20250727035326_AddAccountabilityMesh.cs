using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountabilityMesh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountabilityDelayLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEscalated = table.Column<bool>(type: "bit", nullable: false),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false),
                    DelayDurationHours = table.Column<int>(type: "int", nullable: false),
                    ResponsiblePartyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountabilityMetric = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountabilityDelayLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountabilityDelayLogs");
        }
    }
}
