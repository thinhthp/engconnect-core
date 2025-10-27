using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class CourseReview
    {
        public int CourseReviewId { get; set; }

        public int CourseId { get; set; }

        public string LearnerId { get; set; } = null!;

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual ApplicationUser Learner { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;
    }
}
