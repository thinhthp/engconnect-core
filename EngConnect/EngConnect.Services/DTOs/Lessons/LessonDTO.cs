using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Lessons
{
    public class LessonDTO
    {
        public int LessonId { get; set; }
        public int? ModuleId { get; set; }
        public string Title { get; set; } = "";
        public string? ContentUrl { get; set; }
        public string? LessonType { get; set; }
        public TimeSpan? Duration { get; set; }
        public int Position { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
