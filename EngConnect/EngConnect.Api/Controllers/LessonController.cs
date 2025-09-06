using EngConnect.Services.DTOs.Lessons;
using EngConnect.Services.Services.Lessons;
using EngConnect.Services.Services.UserContext;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EngConnect.Api.Controllers
{
    [Route("api/lessons")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _service;
        private readonly IUserContextService _userContext;

        public LessonController(ILessonService service, IUserContextService userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        // POST: api/lessons
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLessonRequest request)
        {
            var userId = _userContext.GetCurrentUserId() ?? "System";
            var created = await _service.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { lessonId = created.LessonId }, created);
        }

        // GET: api/lessons/module/{moduleId}
        [HttpGet("module/{moduleId:int}")]
        public async Task<IActionResult> GetByModule(int moduleId)
        {
            var lessons = await _service.GetByModuleAsync(moduleId);
            return Ok(lessons);
        }

        // GET: api/lessons/{lessonId}
        [HttpGet("{lessonId:int}")]
        public async Task<IActionResult> GetById(int lessonId)
        {
            var lesson = await _service.GetByIdAsync(lessonId);
            if (lesson == null) return NotFound();
            return Ok(lesson);
        }

        // PUT: api/lessons/{lessonId}
        [HttpPut("{lessonId:int}")]
        public async Task<IActionResult> Update(int lessonId, [FromBody] UpdateLessonRequest request)
        {
            var userId = _userContext.GetCurrentUserId() ?? "System";
            var updated = await _service.UpdateAsync(lessonId, request, userId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/lessons/{lessonId}
        [HttpDelete("{lessonId:int}")]
        public async Task<IActionResult> Delete(int lessonId)
        {
            var userId = _userContext.GetCurrentUserId() ?? "System";
            var ok = await _service.DeleteAsync(lessonId, userId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}