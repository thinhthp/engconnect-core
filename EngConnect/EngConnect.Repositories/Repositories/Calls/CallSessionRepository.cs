using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Calls
{
    public class CallSessionRepository : GenericRepository<CallSession>, ICallSessionRepository
    {
        public CallSessionRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<CallSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.CallSessionId == id, cancellationToken);
        }

        public Task UpdateAsync(CallSession session, CancellationToken cancellationToken = default)
        {
            Update(session);
            return Task.CompletedTask;
        }
    }
}