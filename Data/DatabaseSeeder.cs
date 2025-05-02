using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MVP_Core.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Questions.Any(q => q.ServiceType == "Water Filtration"))
                return; // Already seeded

            using var transaction = context.Database.BeginTransaction();

            try
            {
                var question = new Question
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

                context.Questions.Add(question);
                context.SaveChanges(); // Required to get generated Id

                var options = new List<QuestionOption>
                {
                    new QuestionOption { QuestionId = question.Id, OptionText = "Estimate for new system" },
                    new QuestionOption { QuestionId = question.Id, OptionText = "Repair an existing system" },
                    new QuestionOption { QuestionId = question.Id, OptionText = "Maintenance or filter change" }
                };

                context.QuestionOptions.AddRange(options);
                context.SaveChanges();

                transaction.Commit();

                Console.WriteLine("✅ Water Filtration questions and options seeded successfully.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"❌ Seeding failed: {ex.Message}");
                throw; // Important: rethrow to avoid hiding startup errors
            }
        }
    }
}
