using Microsoft.AspNetCore.Mvc;
using MVP_Core.Data.Models.ViewModels;
using MVP_Core.Services;

namespace MVP_Core.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlumbingQuestionController : ControllerBase
    {
        private readonly QuestionService _questionService;
        private readonly ILogger<PlumbingQuestionController> _logger;

        public PlumbingQuestionController(QuestionService questionService, ILogger<PlumbingQuestionController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionWithOptionsModel>>> GetRootQuestions()
        {
            _logger.LogInformation("Fetching root Plumbing questions...");

            var questions = await _questionService.GetRootQuestionsAsync("Plumbing");

            if (questions == null || !questions.Any())
            {
                _logger.LogWarning("No root Plumbing questions found.");
                return NotFound("No questions found for Plumbing.");
            }

            _logger.LogInformation("Retrieved {Count} root Plumbing questions.", questions.Count);
            return Ok(questions);
        }

        [HttpGet("followup")]
        public async Task<ActionResult<List<QuestionWithOptionsModel>>> GetFollowUps([FromQuery] int parentId, [FromQuery] string answer)
        {
            if (parentId <= 0)
            {
                _logger.LogWarning("Invalid ParentId: {ParentId}", parentId);
                return BadRequest("Valid ParentId is required.");
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                _logger.LogWarning("Empty answer value passed.");
                return BadRequest("Answer parameter is required.");
            }

            _logger.LogInformation("Looking for follow-up questions — ParentId: {ParentId}, Answer: {Answer}", parentId, answer);

            var followUps = await _questionService.GetFollowUpQuestionsAsync(parentId, answer.Trim());

            if (followUps == null || !followUps.Any())
            {
                _logger.LogWarning("No follow-up questions found for ParentId={ParentId} and Answer='{Answer}'.", parentId, answer);
                return NotFound("No follow-up questions found.");
            }

            _logger.LogInformation("Found {Count} follow-up questions.", followUps.Count);
            return Ok(followUps);
        }
    }
}
