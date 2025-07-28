using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class Sprint85_8_PhotoMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GeoLatitude",
                table: "TechnicianMedias",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeoLongitude",
                table: "TechnicianMedias",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhotoTimestamp",
                table: "TechnicianMedias",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeoLatitude",
                table: "TechnicianMedias");

            migrationBuilder.DropColumn(
                name: "GeoLongitude",
                table: "TechnicianMedias");

            migrationBuilder.DropColumn(
                name: "PhotoTimestamp",
                table: "TechnicianMedias");
        }
    }
}
