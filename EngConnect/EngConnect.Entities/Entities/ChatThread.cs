using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class ChatThread
    {
        public int ThreadId { get; set; }
        public string? Title { get; set; }
        public bool IsGroup { get; set; }

        public string? Note { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? CreateBy { get; set; } = "None";
        public string? UpdateBy { get; set; }

        public virtual ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}