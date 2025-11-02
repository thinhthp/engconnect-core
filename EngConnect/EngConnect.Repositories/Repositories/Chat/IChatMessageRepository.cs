using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public interface IChatMessageRepository
    {
        Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, System.DateTime? before = null, CancellationToken cancellationToken = default);
    }
}