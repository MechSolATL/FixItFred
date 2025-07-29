using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace MVP_Core.Migrations
{
    // Sprint 90.1
    public partial class Sprint901_LLMTools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromptVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Version = table.Column<string>(maxLength: 100, nullable: false),
                    PromptText = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_PromptVersions", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "LLMModelProviders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderName = table.Column<string>(maxLength: 100, nullable: false),
                    ModelName = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_LLMModelProviders", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "PromptExperiments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromptVersionId = table.Column<int>(nullable: false),
                    ExperimentName = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    EndedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_PromptExperiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptExperiments_PromptVersions_PromptVersionId",
                        column: x => x.PromptVersionId,
                        principalTable: "PromptVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromptTraceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromptVersionId = table.Column<int>(nullable: false),
                    ExperimentId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    SessionId = table.Column<string>(nullable: false),
                    Input = table.Column<string>(nullable: false),
                    Output = table.Column<string>(nullable: true),
                    TraceJson = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_PromptTraceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptTraceLogs_PromptVersions_PromptVersionId",
                        column: x => x.PromptVersionId,
                        principalTable: "PromptVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromptTraceLogs_PromptExperiments_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "PromptExperiments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromptExperiments_PromptVersionId",
                table: "PromptExperiments",
                column: "PromptVersionId");
            migrationBuilder.CreateIndex(
                name: "IX_PromptTraceLogs_PromptVersionId",
                table: "PromptTraceLogs",
                column: "PromptVersionId");
            migrationBuilder.CreateIndex(
                name: "IX_PromptTraceLogs_ExperimentId",
                table: "PromptTraceLogs",
                column: "ExperimentId");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PromptTraceLogs");
            migrationBuilder.DropTable(name: "PromptExperiments");
            migrationBuilder.DropTable(name: "LLMModelProviders");
            migrationBuilder.DropTable(name: "PromptVersions");
        }
    }
}
