using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Chat
{
    public class ChatThreadRepository : GenericRepository<ChatThread>, IChatThreadRepository
    {
        public ChatThreadRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<ChatThread?> GetDirectThreadForUsersAsync(string userId, string otherUserId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Participants)
                .Where(t => !t.IsGroup)
                .Where(t => t.Participants.Count == 2)
                .FirstOrDefaultAsync(
                    t => t.Participants.Any(p => p.UserId == userId)
                      && t.Participants.Any(p => p.UserId == otherUserId),
                    cancellationToken);
        }

        public async Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Participants)
                .Where(t => t.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(t => t.Messages.Max(m => (DateTime?)m.CreatedAt))
                .ToListAsync(cancellationToken);
        }
    }
}