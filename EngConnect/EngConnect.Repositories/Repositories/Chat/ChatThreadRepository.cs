using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Chat
{
    public class ChatThreadRepository : IChatThreadRepository
    {
        private readonly EngConnectContext _context;

        public ChatThreadRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<ChatThread?> GetDirectThreadForUsersAsync(string userId, string otherUserId, CancellationToken cancellationToken = default)
        {
            return await _context.ChatThreads
                .Include(t => t.Participants)
                .Where(t => !t.IsGroup)
                .Where(t => t.Participants.Count == 2)
                .FirstOrDefaultAsync(
                    t => t.Participants.Any(p => p.UserId == userId)
                      && t.Participants.Any(p => p.UserId == otherUserId),
                    cancellationToken);
        }

        public async Task AddAsync(ChatThread thread, CancellationToken cancellationToken = default)
        {
            await _context.ChatThreads.AddAsync(thread, cancellationToken);
        }

        public async Task<IReadOnlyList<ChatThread>> GetUserThreadsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ChatThreads
                .AsNoTracking()
                .Include(t => t.Participants)
                .Where(t => t.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(t => t.Messages.Max(m => (DateTime?)m.CreatedAt))
                .ToListAsync(cancellationToken);
        }
    }
}