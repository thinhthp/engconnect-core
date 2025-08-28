using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class TutorSchedule
    {
        public int ScheduleId { get; set; }

        public string TutorId { get; set; }

        public int? AvailabilityId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsBooked { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual TutorWeeklyAvailability? Availability { get; set; }

        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

        public virtual TutorProfile Tutor { get; set; } = null!;
    }
}
