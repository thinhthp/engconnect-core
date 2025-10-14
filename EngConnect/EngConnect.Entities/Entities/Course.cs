using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class Course
    {
        public int CourseId { get; set; }

        public string? TutorId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Level { get; set; }

        public int TotalSessions { get; set; }

        public decimal Price { get; set; }

        public string? IntroVideo { get; set; }

        public string Status { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual TutorProfile Tutor { get; set; } = null!;
    }
}
