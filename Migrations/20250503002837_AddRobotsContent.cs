using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRobotsContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ?? Drop unique constraint to allow column alteration
            _ = migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * FROM sys.objects 
                    WHERE type = 'UQ' AND name = 'UQ_AdminUsers_Username'
                )
                BEGIN
                    ALTER TABLE [AdminUsers] DROP CONSTRAINT [UQ_AdminUsers_Username];
                END
            ");

            // ?? Alter columns
            _ = migrationBuilder.AlterColumn<string>(
                name: "OptionValue",
                table: "QuestionOptions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "QuestionOptions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "ReviewNotes",
                table: "ProfileReviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // _ = migrationBuilder.AlterColumn<string>(
            //     name: "Phone",
            //     table: "Customers",
            //     type: "nvarchar(20)",
            //     maxLength: 20,
            //     nullable: false,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "AdminUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AdminUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // ? Re-add unique constraint
            _ = migrationBuilder.Sql(@"
                ALTER TABLE [AdminUsers]
                ADD CONSTRAINT [UQ_AdminUsers_Username] UNIQUE ([Username]);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * FROM sys.objects 
                    WHERE type = 'UQ' AND name = 'UQ_AdminUsers_Username'
                )
                BEGIN
                    ALTER TABLE [AdminUsers] DROP CONSTRAINT [UQ_AdminUsers_Username];
                END
            ");

            _ = migrationBuilder.AlterColumn<string>(
                name: "OptionValue",
                table: "QuestionOptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "QuestionOptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            _ = migrationBuilder.AlterColumn<string>(
                name: "ReviewNotes",
                table: "ProfileReviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "AdminUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AdminUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            _ = migrationBuilder.Sql(@"
                ALTER TABLE [AdminUsers]
                ADD CONSTRAINT [UQ_AdminUsers_Username] UNIQUE ([Username]);
            ");
        }
    }
}
