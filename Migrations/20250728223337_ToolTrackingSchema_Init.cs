using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class ToolTrackingSchema_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToolInventories",
                columns: table => new
                {
                    ToolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssignedTechId = table.Column<int>(type: "int", nullable: true),
                    ConditionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolInventories", x => x.ToolId);
                    table.ForeignKey(
                        name: "FK_ToolInventories_Technicians_AssignedTechId",
                        column: x => x.AssignedTechId,
                        principalTable: "Technicians",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ToolTransferLogs",
                columns: table => new
                {
                    TransferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToolId = table.Column<int>(type: "int", nullable: false),
                    FromTechId = table.Column<int>(type: "int", nullable: false),
                    ToTechId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConfirmedByReceiver = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolTransferLogs", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_ToolTransferLogs_Technicians_FromTechId",
                        column: x => x.FromTechId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToolTransferLogs_Technicians_ToTechId",
                        column: x => x.ToTechId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToolTransferLogs_ToolInventories_ToolId",
                        column: x => x.ToolId,
                        principalTable: "ToolInventories",
                        principalColumn: "ToolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToolInventories_AssignedTechId",
                table: "ToolInventories",
                column: "AssignedTechId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolTransferLogs_FromTechId",
                table: "ToolTransferLogs",
                column: "FromTechId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolTransferLogs_ToolId",
                table: "ToolTransferLogs",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolTransferLogs_ToTechId",
                table: "ToolTransferLogs",
                column: "ToTechId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToolTransferLogs");

            migrationBuilder.DropTable(
                name: "ToolInventories");
        }
    }
}
