using EngConnect.Services.DTOs.Submissions;
using EngConnect.Services.Services.Submissions;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private ISubmissionService _service;

        public SubmissionController(ISubmissionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SubmissionAssignment([FromBody] CreateSubmissionRequest request, CancellationToken cancellationToken = default)
        {
            var result = new SubmissionDTO()
            {
                AssignmentId = request.AssignmentId,
                Content = request.Content,
                Note = request.Note
            };
            await _service.SubmitAsync(result, cancellationToken);

            return CreatedAtAction(nameof(GetSubmissionById), new { id = result.SubmissionId }, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmissionById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetSubmissionById(id, cancellationToken);
            return Ok(result); 
        }
        [HttpGet]
        public async Task<IActionResult> GetSubmissionByLearner(CancellationToken cancellationToken)
        {
            var result = await _service.GetSubmissionByLearner(cancellationToken);
            return Ok(result); 
        }
    }
}