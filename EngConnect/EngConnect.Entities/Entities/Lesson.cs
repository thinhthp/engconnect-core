using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class Lesson
    {
        public int LessonId { get; set; }

        public int? ModuleId { get; set; }

        public string Title { get; set; } = null!;

        public string? ContentUrl { get; set; }

        public string? LessonType { get; set; }

        public TimeSpan? Duration { get; set; }

        public int Position { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual CourseModule? Module { get; set; }
    }
}
