using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Services.DTOs.Availabilities;
using EngConnect.Services.Services.UserContext;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/tutor-availabilities")]
    [ApiController]
    public class TutorAvailabilityController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserContextService _userCtx;

        public TutorAvailabilityController(IUnitOfWork uow, IUserContextService userCtx)
        {
            _uow = uow;
            _userCtx = userCtx;
        }

        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<AvailabilitySlotResponse>>> GetMineAsync(CancellationToken ct)
        {
            var tutorId = _userCtx.GetCurrentUserId();
            if (string.IsNullOrEmpty(tutorId)) return Unauthorized();

            var items = await _uow.TutorWeeklyAvailabilityRepository.GetByTutorAsync(tutorId, ct);
            var result = items.Select(a => new AvailabilitySlotResponse(a.AvailabilityId, a.DayOfWeek, a.StartTime, a.EndTime, a.Note));
            return Ok(result);
        }

        [HttpPost("me/bulk")]
        public async Task<ActionResult> UpsertMineBulkAsync([FromBody] BulkAvailabilityRequest request, CancellationToken ct)
        {
            var tutorId = _userCtx.GetCurrentUserId();
            if (string.IsNullOrEmpty(tutorId)) return Unauthorized();

            // Basic validation
            foreach (var s in request.Slots)
            {
                if (s.DayOfWeek is < 0 or > 6) return BadRequest("DayOfWeek must be in [0..6].");
                if (s.EndTime <= s.StartTime) return BadRequest("EndTime must be greater than StartTime.");
            }

            var existing = await _uow.TutorWeeklyAvailabilityRepository.GetByTutorAsync(tutorId, ct);

            if (request.ReplaceExisting && existing.Count > 0)
            {
                await _uow.TutorWeeklyAvailabilityRepository.RemoveRangeAsync(existing, ct);
            }
            else
            {
                // Prevent overlaps against existing if not replacing
                foreach (var s in request.Slots)
                {
                    var overlaps = await _uow.TutorWeeklyAvailabilityRepository
                        .OverlapsAsync(tutorId, s.DayOfWeek, s.StartTime, s.EndTime, null, ct);
                    if (overlaps) return BadRequest($"Overlapping slot on day {s.DayOfWeek} {s.StartTime}-{s.EndTime}.");
                }
            }

            var toAdd = request.Slots.Select(s => new TutorWeeklyAvailability
            {
                TutorId = tutorId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Note = s.Note,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreateBy = tutorId
            }).ToList();

            await _uow.TutorWeeklyAvailabilityRepository.AddRangeAsync(toAdd, ct);
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("me/{availabilityId:int}")]
        public async Task<ActionResult> DeleteMineAsync([FromRoute] int availabilityId, CancellationToken ct)
        {
            var tutorId = _userCtx.GetCurrentUserId();
            if (string.IsNullOrEmpty(tutorId)) return Unauthorized();

            var item = await _uow.TutorWeeklyAvailabilityRepository.GetByIdAsync(availabilityId, ct);
            if (item == null || item.TutorId != tutorId) return NotFound();

            await _uow.TutorWeeklyAvailabilityRepository.RemoveRangeAsync(new[] { item }, ct);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}