using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public interface IChatMessageRepository : IGenericRepository<ChatMessage>
    {
        Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, System.DateTime? before = null, CancellationToken cancellationToken = default);
    }
}