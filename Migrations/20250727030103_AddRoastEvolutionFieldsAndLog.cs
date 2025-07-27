using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRoastEvolutionFieldsAndLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AIAuthored",
                table: "RoastTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoPromote",
                table: "RoastTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                table: "RoastTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LegacyStatus",
                table: "RoastTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Retired",
                table: "RoastTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "SuccessRate",
                table: "RoastTemplates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "RoastEvolutionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoastTemplateId = table.Column<int>(type: "int", nullable: false),
                    EditType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Editor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectivenessScore = table.Column<double>(type: "float", nullable: false),
                    Promoted = table.Column<bool>(type: "bit", nullable: false),
                    Retired = table.Column<bool>(type: "bit", nullable: false),
                    IsLegacy = table.Column<bool>(type: "bit", nullable: false),
                    IsAIAuthored = table.Column<bool>(type: "bit", nullable: false),
                    PreviousMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoastEvolutionLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoastEvolutionLogs");

            migrationBuilder.DropColumn(
                name: "AIAuthored",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "AutoPromote",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "LegacyStatus",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "Retired",
                table: "RoastTemplates");

            migrationBuilder.DropColumn(
                name: "SuccessRate",
                table: "RoastTemplates");
        }
    }
}
