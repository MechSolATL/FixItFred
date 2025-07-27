using Microsoft.EntityFrameworkCore.Migrations;

namespace MVP_Core.Migrations
{
    public partial class Sprint76_IntegrityMatrix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeOnboardingProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(nullable: false),
                    SubmissionDate = table.Column<DateTime>(nullable: false),
                    TraitScoresJson = table.Column<string>(nullable: false),
                    RawAnswersJson = table.Column<string>(nullable: false),
                    InitialRiskScore = table.Column<double>(nullable: false),
                    ReviewRequired = table.Column<bool>(nullable: false),
                    EntryVector = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeOnboardingProfiles", x => x.Id);
                });

            migrationBuilder.AddColumn<bool>(
                name: "HasCompletedIntegritySurvey",
                table: "EmployeeMilestoneLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EmployeeOnboardingProfiles");
            migrationBuilder.DropColumn(name: "HasCompletedIntegritySurvey", table: "EmployeeMilestoneLogs");
        }
    }
}
