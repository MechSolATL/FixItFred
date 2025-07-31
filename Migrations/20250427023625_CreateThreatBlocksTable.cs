using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class CreateThreatBlocksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "PageVisits");

            _ = migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "PageVisitLogs");

            _ = migrationBuilder.RenameColumn(
                name: "IPAddress",
                table: "PageVisitLogs",
                newName: "IpAddress");

            _ = migrationBuilder.RenameColumn(
                name: "VisitTimeUtc",
                table: "PageVisitLogs",
                newName: "VisitTimestamp");

            _ = migrationBuilder.RenameColumn(
                name: "ReferrerUrl",
                table: "PageVisitLogs",
                newName: "Referrer");

            _ = migrationBuilder.AddColumn<bool>(
                name: "IsRealUser",
                table: "PageVisitLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<int>(
                name: "ResponseStatusCode",
                table: "PageVisitLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.CreateTable(
                name: "ThreatBlocks",
                columns: static table => new
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
                constraints: static table =>
                {
                    _ = table.PrimaryKey("PK_ThreatBlocks", static x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "ThreatBlocks");

            _ = migrationBuilder.DropColumn(
                name: "IsRealUser",
                table: "PageVisitLogs");

            _ = migrationBuilder.DropColumn(
                name: "ResponseStatusCode",
                table: "PageVisitLogs");

            _ = migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "PageVisitLogs",
                newName: "IPAddress");

            _ = migrationBuilder.RenameColumn(
                name: "VisitTimestamp",
                table: "PageVisitLogs",
                newName: "VisitTimeUtc");

            _ = migrationBuilder.RenameColumn(
                name: "Referrer",
                table: "PageVisitLogs",
                newName: "ReferrerUrl");

            _ = migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "PageVisitLogs",
                type: "int",
                nullable: true);

            _ = migrationBuilder.CreateTable(
                name: "PageVisits",
                columns: static table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionInfoId = table.Column<int>(type: "int", nullable: false),
                    UserClick = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: static table =>
                {
                    _ = table.PrimaryKey("PK_PageVisits", static x => x.Id);
                });
        }
    }
}
