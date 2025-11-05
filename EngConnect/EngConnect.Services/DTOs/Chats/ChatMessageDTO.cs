using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Chats
{
    public sealed class ChatMessageDTO
    {
        public long MessageId { get; set; }
        public int ThreadId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}