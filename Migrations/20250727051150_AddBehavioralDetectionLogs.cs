using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddBehavioralDetectionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymousReviewFormLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InitiatorCount = table.Column<int>(type: "int", nullable: false),
                    ContextHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResolutionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousReviewFormLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EgoVectorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    WeekStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverrideCommandCount = table.Column<int>(type: "int", nullable: false),
                    TrustDelayCount = table.Column<int>(type: "int", nullable: false),
                    DisputeReversalCount = table.Column<int>(type: "int", nullable: false),
                    EgoInfluenceScore = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EgoVectorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeConfidenceDecayLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    WeekStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfidenceShift = table.Column<double>(type: "float", nullable: false),
                    PulseScore = table.Column<double>(type: "float", nullable: false),
                    InteractionMatrixScore = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeConfidenceDecayLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymousReviewFormLogs");

            migrationBuilder.DropTable(
                name: "EgoVectorLogs");

            migrationBuilder.DropTable(
                name: "EmployeeConfidenceDecayLogs");
        }
    }
}
