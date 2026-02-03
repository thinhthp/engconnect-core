using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class CallSignal
    {
        public long SignalId { get; set; }

        public long CallSessionId { get; set; }

        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;

        public string Type { get; set; } = null!;       // offer | answer | candidate | renegotiate | hangup | custom

        public string Payload { get; set; } = null!;    // JSON payload (SDP / ICE candidate / ...

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }

        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public virtual CallSession CallSession { get; set; } = null!;
        public virtual ApplicationUser Sender { get; set; } = null!;
        public virtual ApplicationUser Receiver { get; set; } = null!;
    }
}