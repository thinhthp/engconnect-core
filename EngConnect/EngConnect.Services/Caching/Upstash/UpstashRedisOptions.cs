using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Caching.Upstash
{
    public sealed class UpstashRedisOptions
    {
        public string RestUrl { get; set; } = string.Empty;
        public string RestToken { get; set; } = string.Empty;
        public int DefaultTtlSeconds { get; set; } = 1800;
    }
}