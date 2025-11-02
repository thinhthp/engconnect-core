using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class ChatMessage
    {
        public long MessageId { get; set; }
        public int ThreadId { get; set; }
        public string SenderId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? ContentType { get; set; } // "text", "image", etc.
        public bool IsEdited { get; set; }

        public string? Note { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? CreateBy { get; set; } = "None";
        public string? UpdateBy { get; set; }

        public virtual ChatThread Thread { get; set; } = default!;
        public virtual ApplicationUser Sender { get; set; } = default!;
    }
}