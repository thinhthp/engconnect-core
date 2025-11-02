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
    public class ChatParticipantRepository : IChatParticipantRepository
    {
        private readonly EngConnectContext _context;

        public ChatParticipantRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<bool> IsParticipantAsync(int threadId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ChatParticipants.AnyAsync(p => p.ThreadId == threadId && p.UserId == userId, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<ChatParticipant> participants, CancellationToken cancellationToken = default)
        {
            await _context.ChatParticipants.AddRangeAsync(participants, cancellationToken);
        }

        public async Task<ChatParticipant?> GetAsync(int threadId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.ChatParticipants.FirstOrDefaultAsync(p => p.ThreadId == threadId && p.UserId == userId, cancellationToken);
        }

        public Task UpdateAsync(ChatParticipant participant, CancellationToken cancellationToken = default)
        {
            _context.ChatParticipants.Update(participant);
            return Task.CompletedTask;
        }
    }
}