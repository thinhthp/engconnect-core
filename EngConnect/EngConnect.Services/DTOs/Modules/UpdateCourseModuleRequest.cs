using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.Modules
{
    public class UpdateCourseModuleRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool? IsActive { get; set; }
        // If null -> keep existing position
        public int? Position { get; set; }
    }
}
