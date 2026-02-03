using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Calls
{
    public interface ICallSessionRepository
    {
        Task<CallSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task AddAsync(CallSession session, CancellationToken cancellationToken = default);
        Task UpdateAsync(CallSession session, CancellationToken cancellationToken = default);
    }
}
