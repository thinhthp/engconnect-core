using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Calls
{
    public class CallSignalRequest
    {
        public long CallSessionId { get; set; }
        public string ReceiverId { get; set; } = null!;
        public string Type { get; set; } = null!;       // offer | answer | candidate | hangup |
        public string Payload { get; set; } = null!;    // JSON from WebRTC
    }
}
