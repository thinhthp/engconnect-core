using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Caching.RateLimiting
{
    public sealed class RateLimitingOptions
    {
        public int PermitLimit { get; set; } = 3;
        public int WindowSeconds { get; set; } = 10;
    }
}