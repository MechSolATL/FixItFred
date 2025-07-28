using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class Sprint849_TechnicianDropAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<string>(
            //     name: "City",
            //     table: "Technicians",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<int>(
            //     name: "HeatScore",
            //     table: "Technicians",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<int>(
            //     name: "LastKnownHeatScore",
            //     table: "Technicians",
            //     type: "int",
            //     nullable: true);

            // migrationBuilder.AddColumn<DateTime>(
            //     name: "LastReviewedAt",
            //     table: "Technicians",
            //     type: "datetime2",
            //     nullable: true);

            // migrationBuilder.AddColumn<int>(
            //     name: "TierLevel",
            //     table: "Technicians",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<int>(
            //     name: "TotalPoints",
            //     table: "Technicians",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<int>(
            //     name: "TrustScore",
            //     table: "Technicians",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0);

            // migrationBuilder.AddColumn<string>(
            //     name: "ZipCode",
            //     table: "Technicians",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<string>(
            //     name: "Comment",
            //     table: "CustomerReviews",
            //     type: "nvarchar(max)",
            //     nullable: true);

            // migrationBuilder.AddColumn<string>(
            //     name: "CustomerName",
            //     table: "CustomerReviews",
            //     type: "nvarchar(max)",
            //     nullable: true);

            // migrationBuilder.AddColumn<bool>(
            //     name: "IsApproved",
            //     table: "CustomerReviews",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<bool>(
            //     name: "IsPublic",
            //     table: "CustomerReviews",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<int>(
            //     name: "TechnicianId",
            //     table: "CustomerReviews",
            //     type: "int",
            //     nullable: true);

            // migrationBuilder.AddColumn<string>(
            //     name: "Tier",
            //     table: "CustomerReviews",
            //     type: "nvarchar(max)",
            //     nullable: true);

            // migrationBuilder.CreateTable(
            //     name: "TechnicianAlertLogs",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         TechnicianId = table.Column<int>(type: "int", nullable: false),
            //         PreviousScore = table.Column<int>(type: "int", nullable: false),
            //         CurrentScore = table.Column<int>(type: "int", nullable: false),
            //         TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //         Acknowledged = table.Column<bool>(type: "bit", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_TechnicianAlertLogs", x => x.Id);
            //     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianAlertLogs");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "HeatScore",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "LastKnownHeatScore",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "LastReviewedAt",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "TierLevel",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "TrustScore",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "CustomerReviews");
        }
    }
}
