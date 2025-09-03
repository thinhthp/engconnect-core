using EngConnect.Services.DTOs.Courses;
using EngConnect.Services.Services.Courses;
using EngConnect.Services.Services.Courses.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // [Authorize(Roles = "Tutor")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                int id = await _courseService.CreateCourseAsync(request, cancellationToken);
                return Created($"/api/courses/{id}", new { id });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message, field = ex.ParamName });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses(
            [FromQuery] string? search,
            [FromQuery] string? level,
            [FromQuery] string? status,
            [FromQuery] string? tutorId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? isActive,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDir,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var filter = new CourseSearchFilter
            {
                Search = search,
                Level = level,
                Status = status,
                TutorId = tutorId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsActive = isActive,
                SortBy = sortBy,
                SortDir = sortDir,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _courseService.GetCoursesAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
        {
            var course = await _courseService.GetCourseByIdAsync(id, cancellationToken);
            if (course == null) return NotFound();
            return Ok(course);
        }
    }
}
