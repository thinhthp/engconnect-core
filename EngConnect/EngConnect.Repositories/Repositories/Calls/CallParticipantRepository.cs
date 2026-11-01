using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;

namespace EngConnect.Repositories.Repositories.Calls
{
    public class CallParticipantRepository : GenericRepository<CallParticipant>, ICallParticipantRepository
    {
        public CallParticipantRepository(EngConnectContext context) : base(context)
        {
        }
    }
}