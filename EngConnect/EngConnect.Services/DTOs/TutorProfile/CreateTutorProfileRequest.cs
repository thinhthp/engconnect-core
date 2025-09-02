using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.DTOs.TutorProfile
{
    public class CreateTutorProfileRequest
    {
        [Range(0, 80)]
        public int? ExperienceYears { get; set; }

        [MaxLength(4000)]
        public string? Bio { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        // External URL
        public string? CvFile { get; set; }

        public string? DemoVideo { get; set; }
    }
}
