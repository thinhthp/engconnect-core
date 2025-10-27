using EngConnect.Entities.Common;
using EngConnect.Services.DTOs.Reviews;
using EngConnect.Services.Services.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/course-reviews")]
    [ApiController]
    public class CourseReviewsController : ControllerBase
    {
        private readonly ICourseReviewService _service;

        public CourseReviewsController(ICourseReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseReviewRequest request, CancellationToken cancellationToken)
        {
            var dto = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = dto.CourseReviewId }, dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            return Ok(dto);
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var paged = await _service.GetByCourseAsync(courseId, pageNumber, pageSize, cancellationToken);
            return Ok(paged);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var paged = await _service.GetAllAsync(pageNumber, pageSize, cancellationToken);
            return Ok(paged);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCourseReviewRequest request, CancellationToken cancellationToken)
        {
            var dto = await _service.UpdateAsync(request, cancellationToken);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
