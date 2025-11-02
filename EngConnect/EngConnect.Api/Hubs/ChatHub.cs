using EngConnect.Services.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EngConnect.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chat;

        public ChatHub(IChatService chat)
        {
            _chat = chat;
        }

        private string UserId => Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("No user id");

        public async Task JoinThread(int threadId)
        {
            if (!await _chat.IsParticipantAsync(threadId, UserId))
                throw new HubException("Not a participant");

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(threadId));
        }

        public async Task LeaveThread(int threadId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(threadId));
        }

        public async Task<int> CreateDirectThread(string otherUserId)
        {
            var thread = await _chat.GetOrCreateDirectThreadAsync(UserId, otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(thread.ThreadId));
            return thread.ThreadId;
        }

        public async Task SendMessage(int threadId, string content, string? contentType = "text")
        {
            var msg = await _chat.SendMessageAsync(threadId, UserId, content, contentType);
            await Clients.Group(GroupName(threadId)).SendAsync("messageReceived", new
            {
                messageId = msg.MessageId,
                threadId = msg.ThreadId,
                senderId = msg.SenderId,
                content = msg.Content,
                contentType = msg.ContentType,
                createdAt = msg.CreatedAt
            });
        }

        public async Task MarkRead(int threadId, DateTime readAtUtc)
        {
            await _chat.SetLastReadAsync(threadId, UserId, readAtUtc);
        }

        private static string GroupName(int threadId) => $"thread:{threadId}";
    }
}
