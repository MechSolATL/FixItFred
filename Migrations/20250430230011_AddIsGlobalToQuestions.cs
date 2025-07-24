using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVP_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGlobalToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // COMMENTED OUT: Already exists in database — causes SQL error
            // migrationBuilder.AddColumn<bool>(
            //     name: "IsGlobal",
            //     table: "Questions",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // ? This one can still be applied
            _ = migrationBuilder.AddColumn<int>(
                name: "ScoreWeight",
                table: "QuestionOptions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // COMMENTED OUT: Avoid dropping column that already exists manually
            // migrationBuilder.DropColumn(
            //     name: "IsGlobal",
            //     table: "Questions");

            _ = migrationBuilder.DropColumn(
                name: "ScoreWeight",
                table: "QuestionOptions");
        }
    }
}
