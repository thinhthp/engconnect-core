using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class ChatParticipant
    {
        public int ParticipantId { get; set; }
        public int ThreadId { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastReadAt { get; set; }
        public bool IsMuted { get; set; }

        public virtual ChatThread Thread { get; set; } = default!;
        public virtual ApplicationUser User { get; set; } = default!;
    }
}