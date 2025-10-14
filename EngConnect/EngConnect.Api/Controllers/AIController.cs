using EngConnect.Services.DTOs.AI;
using EngConnect.Services.Services.AI;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/ais")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AISimpleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            var response = await _aiService.ChatAsync(request.Message);
            return Ok(new { response });
        }

        [HttpPost("hint")]
        public async Task<IActionResult> GetHint([FromBody] AISimpleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return BadRequest("Question cannot be empty.");
            }

            var response = await _aiService.GetHintAsync(request.Message);
            return Ok(new { response });
        }
    }
}