using EngConnect.Services.DTOs.TutorSchedules;
using EngConnect.Services.Services.TutorSchedules;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/tutor-schedules")]
    [ApiController]
    public class TutorScheduleController : ControllerBase
    {
        private readonly ITutorScheduleService _service;

        public TutorScheduleController(ITutorScheduleService service)
        {
            _service = service;
        }

        // onlyAvailable=true hides already booked slots.
        [HttpGet("by-tutor/{tutorId}")]
        public async Task<ActionResult<IEnumerable<ScheduleSlotResponse>>> GetByTutorAsync(
            [FromRoute] string tutorId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] bool onlyAvailable = true,
            CancellationToken ct = default)
        {
            try
            {
                var result = await _service.GetByTutorAsync(tutorId, from, to, onlyAvailable, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}