using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsForLoadBalancing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredSkills",
                table: "ServiceRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TechnicianLoadLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianLoadLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianLoadLogs_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSkills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianSkillMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSkillMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianSkillMaps_TechnicianSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "TechnicianSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicianSkillMaps_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianLoadLogs_TechnicianId",
                table: "TechnicianLoadLogs",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSkillMaps_SkillId",
                table: "TechnicianSkillMaps",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSkillMaps_TechnicianId",
                table: "TechnicianSkillMaps",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianLoadLogs");

            migrationBuilder.DropTable(
                name: "TechnicianSkillMaps");

            migrationBuilder.DropTable(
                name: "TechnicianSkills");

            migrationBuilder.DropColumn(
                name: "RequiredSkills",
                table: "ServiceRequests");
        }
    }
}
