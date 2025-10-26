using EngConnect.Services.Services.Students;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents(CancellationToken cancellationToken)
        {
            var list = await _studentService.GetStudentsAsync(cancellationToken);
            return Ok(list);
        }
    }
}
