using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Chat
{
    public class ChatParticipantRepository : GenericRepository<ChatParticipant>, IChatParticipantRepository
    {
        public ChatParticipantRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(p => p.ThreadId == threadId && p.UserId == userId, cancellationToken);
        }

        public async Task<ChatParticipant?> GetAsync(int threadId, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.ThreadId == threadId && p.UserId == userId, cancellationToken);
        }

        public Task UpdateAsync(ChatParticipant participant, CancellationToken cancellationToken = default)
        {
            Update(participant);
            return Task.CompletedTask;
        }
    }
}