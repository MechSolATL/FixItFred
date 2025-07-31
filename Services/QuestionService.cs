using Data;
using Data.Models;
using Data.Models.ViewModels;

namespace Services
{
    public class QuestionService
    {
        private readonly ApplicationDbContext _context;

        public QuestionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Question? GetQuestionById(int id)
        {
            return _context.Questions
                .Include(q => q.Options)
                .FirstOrDefault(q => q.Id == id);
        }

        public Question? GetFirstQuestion(string serviceType)
        {
            return _context.Questions
                .Include(q => q.Options)
                .Where(q => q.ServiceType == serviceType && q.ParentQuestionId == null)
                .OrderBy(q => q.Id)
                .FirstOrDefault();
        }

        public Question? GetNextQuestion(string serviceType, int currentQuestionId, string currentAnswer)
        {
            return _context.Questions
                .Include(q => q.Options)
                .Where(q => q.ServiceType == serviceType &&
                            q.ParentQuestionId == currentQuestionId &&
                            q.ExpectedAnswer!.ToLower() == (currentAnswer ?? "").ToLower())
                .OrderBy(q => q.Id)
                .FirstOrDefault();
        }

        public Question? GetPreviousQuestion(string serviceType, int currentQuestionId)
        {
            Question? current = _context.Questions.Find(currentQuestionId);
            return current?.ParentQuestionId == null
                ? null
                : _context.Questions
                .Include(q => q.Options)
                .FirstOrDefault(q => q.Id == current.ParentQuestionId);
        }

        public int GetStepIndex(string serviceType, int questionId)
        {
            List<Question> ordered = _context.Questions
                .Where(q => q.ServiceType == serviceType)
                .OrderBy(q => q.Id)
                .ToList();

            int index = ordered.FindIndex(q => q.Id == questionId);
            return index >= 0 ? index + 1 : 1;
        }

        public int GetQuestionCount(string serviceType)
        {
            return _context.Questions.Count(q => q.ServiceType == serviceType);
        }

        /// <summary>
        /// ?? Load all root-level questions for a given service (e.g., Plumbing)
        /// </summary>
        public async Task<List<QuestionWithOptionsModel>> GetRootQuestionsAsync(string serviceType)
        {
            List<Question> questions = await _context.Questions
                .Where(q => q.ServiceType == serviceType && q.ParentQuestionId == null)
                .OrderBy(q => q.Id)
                .Include(q => q.Options)
                .ToListAsync();

            return questions.Select(q => new QuestionWithOptionsModel
            {
                Id = q.Id,
                GroupName = q.GroupName ?? string.Empty,
                ServiceType = q.ServiceType,
                Text = q.Text,
                InputType = q.InputType,
                IsMandatory = q.IsMandatory,
                ParentQuestionId = q.ParentQuestionId,
                ExpectedAnswer = q.ExpectedAnswer,
                IsPrompt = q.IsPrompt,
                PromptMessage = q.PromptMessage,
                Page = q.Page,
                Options = q.Options?
                    .GroupBy(o => o.OptionText.Trim().ToLower())
                    .Select(g => g.First())
                    .ToList() ?? []
            }).ToList();
        }

        /// <summary>
        /// ?? Load follow-up questions based on parent ID and expected answer
        /// </summary>
        public async Task<List<QuestionWithOptionsModel>> GetFollowUpQuestionsAsync(int parentId, string userAnswer)
        {
            string normalizedAnswer = (userAnswer ?? string.Empty).Trim().ToLower();

            List<Question> childQuestions = await _context.Questions
                .Where(q =>
                    q.ParentQuestionId == parentId &&
                    (q.ExpectedAnswer ?? string.Empty).Trim().ToLower() == normalizedAnswer)
                .OrderBy(q => q.Id)
                .Include(q => q.Options)
                .ToListAsync();

            return childQuestions.Select(q => new QuestionWithOptionsModel
            {
                Id = q.Id,
                GroupName = q.GroupName ?? string.Empty,
                ServiceType = q.ServiceType,
                Text = q.Text,
                InputType = q.InputType,
                IsMandatory = q.IsMandatory,
                ParentQuestionId = q.ParentQuestionId,
                ExpectedAnswer = q.ExpectedAnswer,
                IsPrompt = q.IsPrompt,
                PromptMessage = q.PromptMessage,
                Page = q.Page,
                Options = q.Options?
                    .GroupBy(o => o.OptionText.Trim().ToLower())
                    .Select(g => g.First())
                    .ToList() ?? []
            }).ToList();
        }

        /// <summary>
        /// ?? Fetch all UserResponses for a given SessionID
        /// </summary>
        public async Task<List<UserResponse>> GetUserResponsesAsync(string sessionId)
        {
            return await _context.UserResponses
                .Where(r => r.SessionID == sessionId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// ?? Save a new ServiceRequest record
        /// </summary>
        public void AddServiceRequest(ServiceRequest request)
        {
            _ = _context.ServiceRequests.Add(request);
        }

        /// <summary>
        /// ?? Save a new UserResponse for a given question
        /// </summary>
        public void AddUserResponse(UserResponse response)
        {
            _ = _context.UserResponses.Add(response);
        }

        /// <summary>
        /// ?? Persist all pending database changes
        /// </summary>
        public void SaveChanges()
        {
            _ = _context.SaveChanges();
        }
    }
}
