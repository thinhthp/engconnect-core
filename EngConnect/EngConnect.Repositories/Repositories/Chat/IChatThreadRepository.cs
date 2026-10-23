using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public interface IChatThreadRepository : IGenericRepository<ChatThread>
    {
        Task<ChatThread?> GetDirectThreadForUsersAsync(string userId, string otherUserId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken cancellationToken = default);
    }
}