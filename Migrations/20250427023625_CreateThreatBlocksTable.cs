using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateThreatBlocksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageVisits");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "PageVisitLogs");

            migrationBuilder.RenameColumn(
                name: "IPAddress",
                table: "PageVisitLogs",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "VisitTimeUtc",
                table: "PageVisitLogs",
                newName: "VisitTimestamp");

            migrationBuilder.RenameColumn(
                name: "ReferrerUrl",
                table: "PageVisitLogs",
                newName: "Referrer");

            migrationBuilder.AddColumn<bool>(
                name: "IsRealUser",
                table: "PageVisitLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResponseStatusCode",
                table: "PageVisitLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ThreatBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StrikeCount = table.Column<int>(type: "int", nullable: false),
                    IsPermanentlyBlocked = table.Column<bool>(type: "bit", nullable: false),
                    FirstDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BanLiftTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatBlocks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreatBlocks");

            migrationBuilder.DropColumn(
                name: "IsRealUser",
                table: "PageVisitLogs");

            migrationBuilder.DropColumn(
                name: "ResponseStatusCode",
                table: "PageVisitLogs");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "PageVisitLogs",
                newName: "IPAddress");

            migrationBuilder.RenameColumn(
                name: "VisitTimestamp",
                table: "PageVisitLogs",
                newName: "VisitTimeUtc");

            migrationBuilder.RenameColumn(
                name: "Referrer",
                table: "PageVisitLogs",
                newName: "ReferrerUrl");

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "PageVisitLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PageVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionInfoId = table.Column<int>(type: "int", nullable: false),
                    UserClick = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageVisits", x => x.Id);
                });
        }
    }
}
