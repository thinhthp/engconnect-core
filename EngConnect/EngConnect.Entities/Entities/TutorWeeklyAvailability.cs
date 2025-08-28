using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class TutorWeeklyAvailability
    {
        public int AvailabilityId { get; set; }

        public string TutorId { get; set; }

        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual TutorProfile Tutor { get; set; } = null!;

        public virtual ICollection<TutorSchedule> Tutorschedules { get; set; } = new List<TutorSchedule>();
    }
}
