using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Courses
{
    public class CreateCourseRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Level { get; set; }
        public int TotalSessions { get; set; }
        public decimal Price { get; set; }
        public string? IntroVideo { get; set; }
        public string? Note { get; set; }
    }
}
