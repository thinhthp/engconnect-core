using EngConnect.Services.DTOs.Calls;
using EngConnect.Services.Services.Calls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EngConnect.Api.Controllers
{
    [Route("api/calls/recordings")]
    [ApiController]
    [Authorize]
    public class CallRecordingController : ControllerBase
    {
        private readonly ICallRecordingService _recordingService;

        public CallRecordingController(ICallRecordingService recordingService)
        {
            _recordingService = recordingService;
        }

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        /// <summary>
        /// Uploads a chunk of a call recording. The chunk is identified by its index and associated with a specific call session.
        /// </summary>
        /// <param name="callSessionId"></param>
        /// <param name="chunkIndex"></param>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("chunk")]
        [RequestSizeLimit(200_000_000)] // adjust as needed
        public async Task<IActionResult> UploadChunk([FromForm] UploadCallChunkRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "File is required." });
            }

            await _recordingService.UploadChunkAsync(CurrentUserId, request, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Completes the recording process for a call session by merging all uploaded chunks into a single file.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("complete")]
        public async Task<IActionResult> Complete([FromBody] CompleteRecordingRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.CallSessionId <= 0)
                return BadRequest(new { message = "CallSessionId is required." });

            var path = await _recordingService.CompleteRecordingAsync(CurrentUserId, request, cancellationToken);
            return Ok(new { recordingFilePath = path });
        }
    }
}