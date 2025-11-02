using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public interface IChatParticipantRepository
    {
        Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<ChatParticipant> participants, CancellationToken cancellationToken = default);
        Task<ChatParticipant?> GetAsync(int threadId, string userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(ChatParticipant participant, CancellationToken cancellationToken = default);
    }
}