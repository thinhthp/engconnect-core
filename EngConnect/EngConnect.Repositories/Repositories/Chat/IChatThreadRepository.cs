using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public interface IChatThreadRepository
    {
        Task<ChatThread?> GetDirectThreadForUsersAsync(string userId, string otherUserId, CancellationToken cancellationToken = default);
        Task AddAsync(ChatThread thread, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken cancellationToken = default);
    }
}