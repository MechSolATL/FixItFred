using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Controllers
{
    [ApiController]
    [Route("api/session")]
    public class SessionController : ControllerBase
    {
        [HttpPost("save-answer")]
        [ValidateAntiForgeryToken] // Only remove if fully API stateless
        public IActionResult SaveAnswer([FromBody] UserAnswerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sessionKey = $"Answer_{model.QuestionId}";
            HttpContext.Session.SetString(sessionKey, model.Answer ?? string.Empty);

            return Ok();
        }

        [HttpGet("load-answers")]
        public IActionResult LoadAnswers()
        {
            var savedAnswers = new Dictionary<string, string>();

            foreach (var key in HttpContext.Session.Keys)
            {
                if (key.StartsWith("Answer_"))
                {
                    savedAnswers[key.Replace("Answer_", "")] = HttpContext.Session.GetString(key) ?? string.Empty;
                }
            }

            return Ok(savedAnswers);
        }

        public class UserAnswerModel
        {
            [Required]
            public string QuestionId { get; set; } = string.Empty;

            public string? Answer { get; set; }
        }
    }
}
