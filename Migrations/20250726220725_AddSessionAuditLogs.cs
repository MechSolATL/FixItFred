using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeatmapJson",
                table: "SlaDriftSnapshots");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.DropColumn(
                name: "Module",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.RenameColumn(
                name: "TotalRequests",
                table: "SlaDriftSnapshots",
                newName: "ViolatedCount");

            migrationBuilder.RenameColumn(
                name: "DriftedRequests",
                table: "SlaDriftSnapshots",
                newName: "TotalJobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SystemSnapshotLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SnapshotDate",
                table: "SlaDriftSnapshots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AffectedModule",
                table: "RootCauseCorrelationLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LoggedAt",
                table: "RootCauseCorrelationLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OccurrenceCount",
                table: "RootCauseCorrelationLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RootCauseLabel",
                table: "RootCauseCorrelationLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Severity",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlertType",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AdminAlertLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DisputeInsightLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisputeId = table.Column<int>(type: "int", nullable: false),
                    InsightType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoggedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeInsightLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionPlaybackLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionPlaybackLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisputeInsightLogs");

            migrationBuilder.DropTable(
                name: "SessionPlaybackLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SystemSnapshotLogs");

            migrationBuilder.DropColumn(
                name: "SnapshotDate",
                table: "SlaDriftSnapshots");

            migrationBuilder.DropColumn(
                name: "AffectedModule",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.DropColumn(
                name: "LoggedAt",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.DropColumn(
                name: "OccurrenceCount",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.DropColumn(
                name: "RootCauseLabel",
                table: "RootCauseCorrelationLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AdminAlertLogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AdminAlertLogs");

            migrationBuilder.RenameColumn(
                name: "ViolatedCount",
                table: "SlaDriftSnapshots",
                newName: "TotalRequests");

            migrationBuilder.RenameColumn(
                name: "TotalJobs",
                table: "SlaDriftSnapshots",
                newName: "DriftedRequests");

            migrationBuilder.AddColumn<string>(
                name: "HeatmapJson",
                table: "SlaDriftSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "RootCauseCorrelationLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Module",
                table: "RootCauseCorrelationLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "RootCauseCorrelationLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Severity",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AlertType",
                table: "AdminAlertLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
