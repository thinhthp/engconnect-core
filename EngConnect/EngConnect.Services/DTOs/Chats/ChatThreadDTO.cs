using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Chats
{
    public sealed class ChatThreadDTO
    {
        public int ThreadId { get; set; }
        public string? Title { get; set; }
        public bool IsGroup { get; set; }
        public string[] ParticipantUserIds { get; set; } = [];
        public string? OtherUserId { get; set; } // For 1-1: convenience for FE
        public DateTime CreatedAt { get; set; }
    }
}