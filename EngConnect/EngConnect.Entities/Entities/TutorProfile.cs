using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class TutorProfile
    {
        public string TutorId { get; set; }

        public int? ExperienceYears { get; set; }

        public string? Bio { get; set; }

        public string? Language { get; set; }

        public string? CvFile { get; set; }

        public string? DemoVideo { get; set; }

        public bool Approved { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

        public virtual ApplicationUser Tutor { get; set; } = null!;

        public virtual ICollection<TutorSchedule> TutorSchedules { get; set; } = new List<TutorSchedule>();

        public virtual ICollection<TutorWeeklyAvailability> TutorWeeklyAvailabilities { get; set; } = new List<TutorWeeklyAvailability>();

        public virtual ICollection<WithdrawRequest> WithdrawRequests { get; set; } = new List<WithdrawRequest>();
    }
}
