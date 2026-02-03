using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Calls
{
    public class CallStatusChangeRequest
    {
        public long CallSessionId { get; set; }
        public string NewStatus { get; set; } = null!;  // accepted | rejected | ended |
    }
}
