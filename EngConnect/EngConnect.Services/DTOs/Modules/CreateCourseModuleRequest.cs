using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Modules
{
    public class CreateCourseModuleRequest
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Note { get; set; }
        // Bam them cuoi khoi kem pos, them giua thi kem pos
        public int? Position { get; set; }
    }
}
