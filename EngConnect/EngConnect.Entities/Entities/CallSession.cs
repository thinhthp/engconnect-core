using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class CallSession
    {
        public long CallSessionId { get; set; }

        public string CallerId { get; set; } = null!;
        public string CalleeId { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }

        public string Status { get; set; } = "pending"; // pending, ringing, active, ended, missed, rejected

        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }

        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public virtual ApplicationUser Caller { get; set; } = null!;
        public virtual ApplicationUser Callee { get; set; } = null!;

        public virtual ICollection<CallParticipant> Participants { get; set; } = new HashSet<CallParticipant>();
        public virtual ICollection<CallSignal> Signals { get; set; } = new HashSet<CallSignal>();
    }
}