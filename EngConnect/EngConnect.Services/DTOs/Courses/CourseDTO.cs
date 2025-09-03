using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Courses
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string TutorId { get; set; }
        public string Title { get; set; } = null!;
        public string? Level { get; set; }
        public int TotalSessions { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
