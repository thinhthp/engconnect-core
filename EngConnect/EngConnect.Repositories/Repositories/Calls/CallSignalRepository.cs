using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;

namespace EngConnect.Repositories.Repositories.Calls
{
    public class CallSignalRepository : GenericRepository<CallSignal>, ICallSignalRepository
    {
        public CallSignalRepository(EngConnectContext context) : base(context)
        {
        }
    }
}