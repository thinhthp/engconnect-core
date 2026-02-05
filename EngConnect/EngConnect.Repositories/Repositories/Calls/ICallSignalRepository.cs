using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Calls
{
    public interface ICallSignalRepository
    {
        Task AddAsync(CallSignal signal, CancellationToken cancellationToken = default);
    }
}