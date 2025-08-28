using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class Submission
    {
        public int SubmissionId { get; set; }

        public int AssignmentId { get; set; }

        public string LearnerId { get; set; }

        public string? Content { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public string Status { get; set; } = null!;

        public decimal? Score { get; set; }

        public string? Feedback { get; set; }

        public DateTime? GradedAt { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;

        public virtual ApplicationUser Learner { get; set; } = null!;
    }
}
