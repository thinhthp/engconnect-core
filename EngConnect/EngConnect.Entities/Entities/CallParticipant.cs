using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class CallParticipant
    {
        public long ParticipantId { get; set; }

        public long CallSessionId { get; set; }
        public string UserId { get; set; } = null!;

        public bool IsCaller { get; set; }
        public bool IsConnected { get; set; }
        public bool IsMuted { get; set; }
        public bool IsVideoEnabled { get; set; } = true;

        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset? LeftAt { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }

        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public virtual CallSession CallSession { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}