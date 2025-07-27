using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddGpsDriftAndSlaMissAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GpsDriftEventLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ScheduledLatitude = table.Column<double>(type: "float", nullable: false),
                    ScheduledLongitude = table.Column<double>(type: "float", nullable: false),
                    ActualLatitude = table.Column<double>(type: "float", nullable: false),
                    ActualLongitude = table.Column<double>(type: "float", nullable: false),
                    DriftDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceDriftMeters = table.Column<double>(type: "float", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    SeverityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResolutionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpsDriftEventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GpsDriftEventLogs_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlaMissAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    ScheduledArrival = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    SlaViolated = table.Column<bool>(type: "bit", nullable: false),
                    ViolationMinutes = table.Column<int>(type: "int", nullable: false),
                    AutoFlagged = table.Column<bool>(type: "bit", nullable: false),
                    AlertSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaMissAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlaMissAlerts_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SlaMissAlerts_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GpsDriftEventLogs_TechnicianId",
                table: "GpsDriftEventLogs",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_SlaMissAlerts_ServiceRequestId",
                table: "SlaMissAlerts",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SlaMissAlerts_TechnicianId",
                table: "SlaMissAlerts",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GpsDriftEventLogs");

            migrationBuilder.DropTable(
                name: "SlaMissAlerts");
        }
    }
}
