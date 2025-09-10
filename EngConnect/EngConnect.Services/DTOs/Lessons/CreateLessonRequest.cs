using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Lessons
{
    public class CreateLessonRequest
    {
        public int ModuleId { get; set; }
        public string Title { get; set; } = "";
        public string? ContentUrl { get; set; }
        public string? LessonType { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? Note { get; set; }
        public int? Position { get; set; } // optional desired insertion position (1-based)
    }
}
