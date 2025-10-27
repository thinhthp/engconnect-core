using EngConnect.Services.DTOs.Account;
using EngConnect.Services.Services.Tutors;
using EngConnect.Services.Services.TutorProfile.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/tutors")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutorService _tutorService;

        public TutorController(ITutorService tutorService)
        {
            _tutorService = tutorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTutors(
            [FromQuery] string? search,
            [FromQuery] string? language,
            [FromQuery] string? sortBy = "createdAt",
            [FromQuery] string? sortDir = "desc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var filter = new TutorProfileSearchFilter
            {
                Search = search,
                Language = language,
                SortBy = sortBy,
                SortDir = sortDir,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _tutorService.GetTutorsAsync(filter, cancellationToken);
            return Ok(result);
        }
    }
}
