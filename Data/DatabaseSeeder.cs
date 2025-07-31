using Data.Models;

namespace Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Questions.Any(static q => q.ServiceType == "Water Filtration"))
            {
                return; // Already seeded
            }

            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction();

            try
            {
                Question question = new()
                {
                    GroupName = "Initial Intake",
                    ServiceType = "Water Filtration",
                    Text = "What type of service are you requesting?",
                    InputType = "Radio",
                    IsMandatory = true,
                    PromptMessage = "Select an option so we can tailor the questions to your needs.",
                    IsPrompt = true,
                    Page = "Water Filtration"
                };

                _ = context.Questions.Add(question);
                _ = context.SaveChanges(); // Required to get generated Id

                var questionId = question.Id; // Sprint 79.7: DatabaseSeeder cleanup - ensure non-nullable
                List<QuestionOption> options = new()
                {
                    new QuestionOption { QuestionId = questionId, OptionText = "Estimate for new system" },
                    new QuestionOption { QuestionId = questionId, OptionText = "Repair an existing system" },
                    new QuestionOption { QuestionId = questionId, OptionText = "Maintenance or filter change" }
                };

                context.QuestionOptions.AddRange(options);
                _ = context.SaveChanges();

                transaction.Commit();

                Console.WriteLine("? Water Filtration questions and options seeded successfully.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"? Seeding failed: {ex.Message}");
                throw; // Important: rethrow to avoid hiding startup errors
            }
        }
    }
}
