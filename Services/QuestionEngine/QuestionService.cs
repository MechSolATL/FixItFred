using System.Collections.Generic;

namespace MVP_Core.Services.QuestionEngine
{
    public class QuestionService : IQuestionService
    {
        public List<string> GetScenarioQuestions(string serviceType)
        {
            return serviceType switch {
                "Plumbing" => new List<string> { "Have you had leaks before?" },
                _ => new List<string> { "Any previous service?" }
            };
        }
    }
    // [Sprint92_02] Added by FixItFred on behalf of Nova
}
