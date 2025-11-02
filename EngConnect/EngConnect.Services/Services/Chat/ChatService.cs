using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _uow;

        public ChatService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChatThread> GetOrCreateDirectThreadAsync(string userId, string otherUserId, CancellationToken ct = default)
        {
            if (userId == otherUserId) throw new InvalidOperationException("Cannot create direct chat with self.");

            var thread = await _uow.ChatThreadRepository.GetDirectThreadForUsersAsync(userId, otherUserId, ct);
            if (thread != null) return thread;

            thread = new ChatThread { IsGroup = false, CreateBy = userId };
            await _uow.ChatThreadRepository.AddAsync(thread, ct);
            await _uow.SaveChangesAsync();

            await _uow.ChatParticipantRepository.AddRangeAsync(new[]
            {
                new ChatParticipant { ThreadId = thread.ThreadId, UserId = userId },
                new ChatParticipant { ThreadId = thread.ThreadId, UserId = otherUserId }
            }, ct);
            await _uow.SaveChangesAsync();

            return thread;
        }

        public Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken ct = default)
        {
            return _uow.ChatParticipantRepository.IsParticipantAsync(threadId, userId, ct);
        }

        public async Task<ChatMessage> SendMessageAsync(int threadId, string senderId, string content, string? contentType = "text", CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("content required");

            var isMember = await IsParticipantAsync(threadId, senderId, ct);
            if (!isMember) throw new UnauthorizedAccessException("Not a participant of this thread.");

            var msg = new ChatMessage
            {
                ThreadId = threadId,
                SenderId = senderId,
                Content = content,
                ContentType = contentType ?? "text",
                CreateBy = senderId
            };

            await _uow.ChatMessageRepository.AddAsync(msg, ct);
            await _uow.SaveChangesAsync();
            return msg;
        }

        public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, DateTime? before = null, CancellationToken ct = default)
        {
            return _uow.ChatMessageRepository.GetMessagesAsync(threadId, take, before, ct);
        }

        public Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken ct = default)
        {
            return _uow.ChatThreadRepository.GetUserThreadsAsync(userId, ct);
        }

        public async Task SetLastReadAsync(int threadId, string userId, DateTime readAt, CancellationToken ct = default)
        {
            var part = await _uow.ChatParticipantRepository.GetAsync(threadId, userId, ct);
            if (part == null) return;

            part.LastReadAt = readAt;
            await _uow.ChatParticipantRepository.UpdateAsync(part, ct);
            await _uow.SaveChangesAsync();
        }
    }
}