using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Calls
{
    public class CallSessionRepository : ICallSessionRepository
    {
        private readonly EngConnectContext _context;

        public CallSessionRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<CallSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.CallSessions
                .FirstOrDefaultAsync(s => s.CallSessionId == id, cancellationToken);
        }

        public async Task AddAsync(CallSession session, CancellationToken cancellationToken = default)
        {
            await _context.CallSessions.AddAsync(session, cancellationToken);
        }

        public Task UpdateAsync(CallSession session, CancellationToken cancellationToken = default)
        {
            _context.CallSessions.Update(session);
            return Task.CompletedTask;
        }
    }
}