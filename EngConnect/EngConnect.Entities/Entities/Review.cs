using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int SessionId { get; set; }

        public string LearnerId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual ApplicationUser Learner { get; set; } = null!;

        public virtual Session Session { get; set; } = null!;
    }
}
