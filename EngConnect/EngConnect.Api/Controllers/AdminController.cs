using EngConnect.Services.Services.Admin;
using EngConnect.Services.Services.Payments;
using EngConnect.Services.Services.Payments.Filters;
using EngConnect.Services.Services.TutorProfile;
using EngConnect.Services.Services.TutorProfile.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ITutorProfileService _tutorProfileService;
        private readonly IPaymentService _paymentService;

        public AdminController(IAdminService adminService, ITutorProfileService tutorProfileService, IPaymentService paymentService)
        {
            _adminService = adminService;
            _tutorProfileService = tutorProfileService;
            _paymentService = paymentService;
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

        // [Authorize(Roles = "Admin")]
        [HttpPut("users/{userId}/ban")]
        public async Task<IActionResult> BanUser(string userId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.BanUserAsync(userId, cancellationToken);
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
        [HttpPut("users/{userId}/unban")]
        public async Task<IActionResult> UnbanUser(string userId, CancellationToken cancellationToken)
        {
            try
            {
                await _adminService.UnbanUserAsync(userId, cancellationToken);
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
        [HttpGet("tutors")]
        public async Task<IActionResult> GetTutorProfiles(
            [FromQuery] string? search,
            [FromQuery] bool? approved,
            [FromQuery] bool? isActive,
            [FromQuery] string? language,
            [FromQuery] string? tutorId,
            [FromQuery] string? sortBy = "createdAt",
            [FromQuery] string? sortDir = "desc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var filter = new TutorProfileSearchFilter
            {
                Search = search,
                Approved = approved,
                IsActive = isActive,
                Language = language,
                TutorId = tutorId,
                SortBy = sortBy,
                SortDir = sortDir,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _tutorProfileService.GetForAdminAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments(
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var filter = new PaymentSearchFilter
            {
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _paymentService.GetForAdminAsync(filter, cancellationToken);
            return Ok(result);
        }
    }
}