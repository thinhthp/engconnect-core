using EngConnect.Services.DTOs.Sessions;
using EngConnect.Services.Services.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace EngConnect.Api.Controllers;
[Route("api/sessions")]
[ApiController]
public class SessionController : ControllerBase
{
     private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

      
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request, CancellationToken cancellationToken)
        {
                var sessionDto = await _sessionService.CreateSession(request, cancellationToken);
                return CreatedAtAction(nameof(GetSessionById), new { sessionId = sessionDto.SessionId }, sessionDto);
            
        }

    
        [HttpPut("{sessionId}/cancel")]
        public async Task<IActionResult> CancelSession(int sessionId, CancellationToken cancellationToken)
        {
          
                var sessionDto = await _sessionService.CancelSession(sessionId, cancellationToken);
                return Ok(sessionDto);
           
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllSessions(CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetAllSessions(cancellationToken);
            return Ok(sessions);
        }

        
        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSessionById(int sessionId, CancellationToken cancellationToken)
        {
            try
            {
                var sessionDto = await _sessionService.GetSessionById(sessionId, cancellationToken);
                return Ok(sessionDto);
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { message = knfEx.Message });
            }
        }
}