using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmailVerificationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.RenameColumn(
                name: "VerificationCode",
                table: "EmailVerifications",
                newName: "Code");

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "EmailVerifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "Expiration",
                table: "EmailVerifications");

            _ = migrationBuilder.RenameColumn(
                name: "Code",
                table: "EmailVerifications",
                newName: "VerificationCode");
        }
    }
}
