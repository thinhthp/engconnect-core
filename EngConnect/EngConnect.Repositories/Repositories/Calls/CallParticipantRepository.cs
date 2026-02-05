using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Calls
{
    public class CallParticipantRepository : ICallParticipantRepository
    {
        private readonly EngConnectContext _context;

        public CallParticipantRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<CallParticipant> participants, CancellationToken cancellationToken = default)
        {
            await _context.CallParticipants.AddRangeAsync(participants, cancellationToken);
        }
    }
}