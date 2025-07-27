using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddReputationGraphAndFusionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisputeFusionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisputeId = table.Column<int>(type: "int", nullable: false),
                    TriggerTechnicianId = table.Column<int>(type: "int", nullable: true),
                    TriggerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TracebackJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutoResolved = table.Column<bool>(type: "bit", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeFusionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianReputationEdges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceTechnicianId = table.Column<int>(type: "int", nullable: false),
                    TargetTechnicianId = table.Column<int>(type: "int", nullable: false),
                    TrustWeight = table.Column<double>(type: "float", nullable: false),
                    InfluenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianReputationEdges", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisputeFusionLogs");

            migrationBuilder.DropTable(
                name: "TechnicianReputationEdges");
        }
    }
}
