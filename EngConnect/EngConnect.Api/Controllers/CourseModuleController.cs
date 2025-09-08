using EngConnect.Services.DTOs.Modules;
using EngConnect.Services.Services.Modules;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EngConnect.Api.Controllers
{
    [Route("api/course-modules")]
    [ApiController]
    public class CourseModuleController : ControllerBase
    {
        private readonly ICourseModuleService _service;

        public CourseModuleController(ICourseModuleService service)
        {
            _service = service;
        }

        // POST: api/course-modules
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseModuleRequest request)
        {
            var userId = User?.Identity?.Name ?? "System";
            var created = await _service.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { moduleId = created.ModuleId }, created);
        }

        // GET: api/course-modules/course/{courseId}
        [HttpGet("course/{courseId:int}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var modules = await _service.GetByCourseAsync(courseId);
            return Ok(modules);
        }

        // GET: api/course-modules/{moduleId}
        [HttpGet("{moduleId:int}")]
        public async Task<IActionResult> GetById(int moduleId)
        {
            var module = await _service.GetByIdAsync(moduleId);
            if (module == null) return NotFound();
            return Ok(module);
        }

        // PUT: api/course-modules/{moduleId}
        [HttpPut("{moduleId:int}")]
        public async Task<IActionResult> Update(int moduleId, [FromBody] UpdateCourseModuleRequest request)
        {
            var userId = User?.Identity?.Name ?? "System";
            var updated = await _service.UpdateAsync(moduleId, request, userId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/course-modules/{moduleId}
        [HttpDelete("{moduleId:int}")]
        public async Task<IActionResult> Delete(int moduleId)
        {
            var userId = User?.Identity?.Name ?? "System";
            var ok = await _service.DeleteAsync(moduleId, userId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}