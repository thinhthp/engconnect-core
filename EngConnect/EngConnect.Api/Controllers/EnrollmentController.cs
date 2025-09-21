using EngConnect.Services.DTOs.Enrollments;
using EngConnect.Services.Services.Enrollments;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollmentByLearnerId([FromQuery] int pageNumber,[FromQuery] int pageSize, CancellationToken cancellationToken = default)
        {
            var response = await _service.GetEnrollmentByLearnerId(pageSize, pageNumber, cancellationToken);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
        {
          
            var enrollment = await _service.CreateEnrollment(request, cancellationToken);
            return CreatedAtAction(nameof(GetEnrollmentByLearnerId), new { id = enrollment.Id }, enrollment);
           
        }

    }
}