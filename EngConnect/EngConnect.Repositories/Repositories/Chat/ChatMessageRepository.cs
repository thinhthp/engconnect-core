using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Chat
{
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, DateTime? before = null, CancellationToken cancellationToken = default)
        {
            var q = _dbSet
                .AsNoTracking()
                .Where(m => m.ThreadId == threadId)
                .OrderByDescending(m => m.CreatedAt);

            if (before.HasValue)
                q = q.Where(m => m.CreatedAt < before.Value).OrderByDescending(m => m.CreatedAt);

            return await q
                .Take(Math.Clamp(take, 1, 200))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}