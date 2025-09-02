using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.TutorProfile
{
    public class TutorProfileDTO
    {
        public string TutorId { get; set; } = default!;
        public int? ExperienceYears { get; set; }
        public string? Bio { get; set; }
        public string? Language { get; set; }
        public string? CvFile { get; set; }
        public string? DemoVideo { get; set; }
        public bool Approved { get; set; }
        public bool IsActive { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
