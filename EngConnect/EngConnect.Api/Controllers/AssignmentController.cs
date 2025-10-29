using EngConnect.Services.DTOs.Assignments;
using EngConnect.Services.Services.Assignments;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers
{
 [Route("api/assignments")]
 [ApiController]
         public class AssignmentController : ControllerBase
         {
            private readonly IAssignmentService _service;

         public AssignmentController(IAssignmentService service)
         {
            _service = service;
         }

         [HttpPost]
         public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request, CancellationToken cancellationToken)
         {
                 try
                 {
                    var dto = await _service.CreateAssignment(request, cancellationToken);
                    return CreatedAtAction(nameof(GetAssignmentById), new { id = dto.AssignmentId }, dto);
                 }
                 catch (ArgumentException ex)
                 {
                     return BadRequest(new { message = ex.Message, field = ex.ParamName });
                 }
         }

         [HttpPut("{assignmentId}")]
         public async Task<IActionResult> UpdateAssignment(int assignmentId, [FromBody] UpdateAssignmentRequest request, CancellationToken cancellationToken)
         {
                 try
                 {
                     request.AssignmentId = assignmentId;
                     var dto = await _service.UpdateAssignment(request, cancellationToken);
                     return Ok(dto);
                 }
                 catch (KeyNotFoundException knf)
                 {
                      return NotFound(new { message = knf.Message });
                 }
                 catch (ArgumentException ex)
                 {
                      return BadRequest(new { message = ex.Message, field = ex.ParamName });
                 }
         }

         [HttpGet("{id}")]
         public async Task<IActionResult> GetAssignmentById(int id, CancellationToken cancellationToken)
         {
                 try
                 {
                     var dto = await _service.GetAssignmentById(id, cancellationToken);
                     return Ok(dto);
                 }
                 catch (KeyNotFoundException knf)
                 {
                     return NotFound(new { message = knf.Message });
                 }
         }

         [HttpDelete("{id}")]
         public async Task<IActionResult> DeleteAssignment(int id, CancellationToken cancellationToken)
         {
             try
             {
                 await _service.DeleteAssignment(id, cancellationToken);
                 return NoContent();
             }
             catch (KeyNotFoundException knf)
             {
                 return NotFound(new { message = knf.Message });
             }
         }
         }
        }
