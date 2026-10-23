using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Calls
{
    public interface ICallSessionRepository : IGenericRepository<CallSession>
    {
        Task<CallSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task UpdateAsync(CallSession session, CancellationToken cancellationToken = default);
    }
}
