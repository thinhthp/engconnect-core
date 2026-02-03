using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Calls
{
    public class CallSignalRepository : ICallSignalRepository
    {
        private readonly EngConnectContext _context;

        public CallSignalRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CallSignal signal, CancellationToken cancellationToken = default)
        {
            await _context.CallSignals.AddAsync(signal, cancellationToken);
        }
    }
}