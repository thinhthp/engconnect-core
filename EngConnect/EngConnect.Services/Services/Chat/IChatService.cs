using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Chat
{
    public interface IChatService
    {
        Task<ChatThread> GetOrCreateDirectThreadAsync(string userId, string otherUserId, CancellationToken ct = default);
        Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken ct = default);
        Task<ChatMessage> SendMessageAsync(int threadId, string senderId, string content, string? contentType = "text", CancellationToken ct = default);
        Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, DateTime? before = null, CancellationToken ct = default);
        Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken ct = default);
        Task SetLastReadAsync(int threadId, string userId, DateTime readAt, CancellationToken ct = default);
    }
}