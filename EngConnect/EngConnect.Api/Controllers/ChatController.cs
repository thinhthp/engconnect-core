using EngConnect.Services.DTOs.Chats;
using EngConnect.Services.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EngConnect.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chat;

        public ChatController(IChatService chat)
        {
            _chat = chat;
        }

        [HttpGet("threads")]
        public async Task<IActionResult> GetThreads(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("No user id");

            var threads = await _chat.GetUserThreadsAsync(userId, ct);

            var dtos = threads.Select(t =>
            {
                var participantUserIds = t.Participants.Select(p => p.UserId).ToArray();
                var otherUserId = t.IsGroup ? null : participantUserIds.FirstOrDefault(id => id != userId);

                return new ChatThreadDTO
                {
                    ThreadId = t.ThreadId,
                    Title = t.Title,
                    IsGroup = t.IsGroup,
                    ParticipantUserIds = participantUserIds,
                    OtherUserId = otherUserId,
                    CreatedAt = t.CreatedAt
                };
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("threads/{threadId:int}/messages")]
        public async Task<IActionResult> GetMessages([FromRoute] int threadId, [FromQuery] int take = 50, [FromQuery] DateTime? before = null, CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("No user id");

            var messages = await _chat.GetMessagesAsync(threadId, take, before, ct);
            var dtos = messages.Select(m => new ChatMessageDTO
            {
                MessageId = m.MessageId,
                ThreadId = m.ThreadId,
                SenderId = m.SenderId,
                Content = m.Content,
                ContentType = m.ContentType,
                CreatedAt = m.CreatedAt
            });

            return Ok(dtos);
        }

        public sealed class CreateDirectThreadRequest
        {
            public string OtherUserId { get; set; } = string.Empty;
        }

        [HttpPost("threads/direct")]
        public async Task<IActionResult> CreateOrGetDirect([FromBody] CreateDirectThreadRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.OtherUserId))
                return BadRequest("otherUserId is required.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("No user id");
            if (request.OtherUserId == userId)
                return BadRequest("Cannot create direct chat with self.");

            var thread = await _chat.GetOrCreateDirectThreadAsync(userId, request.OtherUserId, ct);

            var participantUserIds = thread.Participants.Select(p => p.UserId).ToArray();
            var otherUserId = thread.IsGroup ? null : participantUserIds.FirstOrDefault(id => id != userId);

            var dto = new ChatThreadDTO
            {
                ThreadId = thread.ThreadId,
                Title = thread.Title,
                IsGroup = thread.IsGroup,
                ParticipantUserIds = participantUserIds,
                OtherUserId = otherUserId,
                CreatedAt = thread.CreatedAt
            };

            return Ok(dto);
        }
    }
}