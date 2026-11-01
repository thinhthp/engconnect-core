using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public interface IChatParticipantRepository : IGenericRepository<ChatParticipant>
    {
        Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken cancellationToken = default);
        Task<ChatParticipant?> GetAsync(int threadId, string userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(ChatParticipant participant, CancellationToken cancellationToken = default);
    }
}