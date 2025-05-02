using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Data.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

public class QuestionService
{
    private readonly ApplicationDbContext _context;

    public QuestionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 🧲 Load all root-level questions for a given service (e.g., Plumbing)
    /// </summary>
    public async Task<List<QuestionWithOptionsModel>> GetRootQuestionsAsync(string serviceType)
    {
        var questions = await _context.Questions
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
                .ToList() ?? new List<QuestionOption>()
        }).ToList();
    }

    /// <summary>
    /// 🔄 Load follow-up questions based on parent ID and expected answer
    /// </summary>
    public async Task<List<QuestionWithOptionsModel>> GetFollowUpQuestionsAsync(int parentId, string userAnswer)
    {
        var normalizedAnswer = (userAnswer ?? string.Empty).Trim().ToLower();

        var childQuestions = await _context.Questions
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
                .ToList() ?? new List<QuestionOption>()
        }).ToList();
    }

    /// <summary>
    /// 📝 Fetch all UserResponses for a given SessionID
    /// </summary>
    public async Task<List<UserResponse>> GetUserResponsesAsync(string sessionId)
    {
        return await _context.UserResponses
            .Where(r => r.SessionID == sessionId)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }
}
