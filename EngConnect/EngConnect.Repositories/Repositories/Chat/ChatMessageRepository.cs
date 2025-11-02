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
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly EngConnectContext _context;

        public ChatMessageRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            await _context.ChatMessages.AddAsync(message, cancellationToken);
        }

        public async Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int threadId, int take = 50, DateTime? before = null, CancellationToken cancellationToken = default)
        {
            var q = _context.ChatMessages
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