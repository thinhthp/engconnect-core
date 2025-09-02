using EngConnect.Services.DTOs.TutorProfile;
using EngConnect.Services.Services.TutorProfile;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/tutor-profiles")]
    [ApiController]
    public class TutorProfileController : ControllerBase
    {
        private readonly ITutorProfileService _tutorProfileService;

        public TutorProfileController(ITutorProfileService tutorProfileService)
        {
            _tutorProfileService = tutorProfileService;
        }

        /// <summary>
        /// Lay Profile cua Tutor dang dang nhap
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        {
            var profile = await _tutorProfileService.GetCurrentAsync(cancellationToken);
            if (profile == null)
                return Unauthorized(); // hoac NotFound()

            return Ok(profile);
        }

        /// <summary>
        /// 1 Acc 1 Profile
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTutorProfileRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _tutorProfileService.CreateForCurrentUserAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetMine), new { }, result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(title: "Failed to create tutor profile", detail: ex.Message);
            }
        }
    }
}