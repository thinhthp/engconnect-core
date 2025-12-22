using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Caching.RateLimiting
{
    public interface IRateLimitStore
    {
        Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken = default);
    }
}