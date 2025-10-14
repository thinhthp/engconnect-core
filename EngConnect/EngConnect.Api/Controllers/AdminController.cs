using EngConnect.Services.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost("tutors/{tutorId}/approve")]
        public async Task<IActionResult> ApproveTutor(string tutorId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.ApproveTutorAsync(tutorId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message, field = ex.ParamName });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost("tutors/{tutorId}/reject")]
        public async Task<IActionResult> RejectTutor(string tutorId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.RejectTutorAsync(tutorId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message, field = ex.ParamName });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost("courses/{courseId:int}/approve")]
        public async Task<IActionResult> ApproveCourse(int courseId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.ApproveCourseAsync(courseId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message, field = ex.ParamName });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost("courses/{courseId:int}/reject")]
        public async Task<IActionResult> RejectCourse(int courseId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.RejectCourseAsync(courseId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message, field = ex.ParamName });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}