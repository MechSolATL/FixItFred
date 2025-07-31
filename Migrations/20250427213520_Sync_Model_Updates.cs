using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class Sync_Model_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ServiceRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "ServiceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "ServiceType",
                table: "ServiceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "ServiceSubtype",
                table: "ServiceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ServiceRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ServiceRequests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ServiceRequests",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "ServiceRequests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "ServiceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "ServiceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "ServiceRequests",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BackupLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "BackupLogs",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "BackupSizeMB",
                table: "BackupLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<string>(
                name: "BackupType",
                table: "BackupLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            _ = migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "BackupLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<string>(
                name: "SourceServer",
                table: "BackupLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            _ = migrationBuilder.CreateTable(
                name: "PageVisits",
                columns: static table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionInfoId = table.Column<int>(type: "int", nullable: false),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserClick = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: static table =>
                {
                    _ = table.PrimaryKey("PK_PageVisits", static x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "PageVisits");

            _ = migrationBuilder.DropColumn(
                name: "BackupSizeMB",
                table: "BackupLogs");

            _ = migrationBuilder.DropColumn(
                name: "BackupType",
                table: "BackupLogs");

            _ = migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "BackupLogs");

            _ = migrationBuilder.DropColumn(
                name: "SourceServer",
                table: "BackupLogs");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            _ = migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "ServiceType",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            _ = migrationBuilder.AlterColumn<string>(
                name: "ServiceSubtype",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            _ = migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            _ = migrationBuilder.AlterColumn<string>(
                name: "AssignedTo",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BackupLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "BackupLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);
        }
    }
}
