using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class FinalizeAdminOpsSuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // [FixItFred] Neutralized legacy schema ops. No destructive schema changes performed. All drops/renames are commented for safety.
            // migrationBuilder.DropColumn(
            //     name: "GrowthBytes",
            //     table: "StorageGrowthSnapshots");

            // migrationBuilder.DropColumn(
            //     name: "TotalSizeBytes",
            //     table: "StorageGrowthSnapshots");

            // migrationBuilder.RenameColumn(
            //     name: "CompressionRatio",
            //     table: "StorageGrowthSnapshots",
            //     newName: "UsageMB");

            migrationBuilder.CreateTable(
                name: "AdminAlertAcknowledgeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertId = table.Column<int>(type: "int", nullable: false),
                    AdminUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAlertAcknowledgeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSnapshotLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnapshotType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSnapshotLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminAlertAcknowledgeLogs");

            migrationBuilder.DropTable(
                name: "SystemSnapshotLogs");

            migrationBuilder.RenameColumn(
                name: "UsageMB",
                table: "StorageGrowthSnapshots",
                newName: "CompressionRatio");

            migrationBuilder.AddColumn<long>(
                name: "GrowthBytes",
                table: "StorageGrowthSnapshots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalSizeBytes",
                table: "StorageGrowthSnapshots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
