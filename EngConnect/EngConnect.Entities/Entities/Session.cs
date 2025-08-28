using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class Session
    {
        public int SessionId { get; set; }

        public int EnrollmentId { get; set; }

        public int? ScheduleId { get; set; }

        public int SessionNumber { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? MeetingLink { get; set; }

        public string Status { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual Enrollment Enrollment { get; set; } = null!;

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual TutorSchedule? Schedule { get; set; }
    }
}
