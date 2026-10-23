using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Chat;
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
        private readonly IChatThreadRepository _chatThreadRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatMessageRepository _chatMessageRepository;

        public ChatService(IUnitOfWork uow, IChatThreadRepository chatThreadRepository, IChatParticipantRepository chatParticipantRepository, IChatMessageRepository chatMessageRepository)
        {
            _uow = uow;
            _chatThreadRepository = chatThreadRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task<ChatThread> GetOrCreateDirectThreadAsync(string userId, string otherUserId, CancellationToken ct = default)
        {
            if (userId == otherUserId) throw new InvalidOperationException("Cannot create direct chat with self.");

            var thread = await _chatThreadRepository.GetDirectThreadForUsersAsync(userId, otherUserId, ct);
            if (thread != null) return thread;

            thread = new ChatThread { IsGroup = false, CreateBy = userId };
            await _chatThreadRepository.AddAsync(thread, ct);
            await _uow.SaveChangesAsync();

            await _chatParticipantRepository.AddRangeAsync(new[]
            {
                new ChatParticipant { ThreadId = thread.ThreadId, UserId = userId },
                new ChatParticipant { ThreadId = thread.ThreadId, UserId = otherUserId }
            }, ct);
            await _uow.SaveChangesAsync();

            return thread;
        }

        public Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken ct = default)
        {
            return _chatParticipantRepository.IsParticipantAsync(threadId, userId, ct);
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

            await _chatMessageRepository.AddAsync(msg, ct);
            await _uow.SaveChangesAsync();
            return msg;
        }

        public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, DateTime? before = null, CancellationToken ct = default)
        {
            return _chatMessageRepository.GetMessagesAsync(threadId, take, before, ct);
        }

        public Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken ct = default)
        {
            return _chatThreadRepository.GetUserThreadsAsync(userId, ct);
        }

        public async Task SetLastReadAsync(int threadId, string userId, DateTime readAt, CancellationToken ct = default)
        {
            var part = await _chatParticipantRepository.GetAsync(threadId, userId, ct);
            if (part == null) return;

            part.LastReadAt = readAt;
            await _chatParticipantRepository.UpdateAsync(part, ct);
            await _uow.SaveChangesAsync();
        }
    }
}