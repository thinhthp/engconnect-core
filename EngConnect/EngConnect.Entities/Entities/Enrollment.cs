using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EngConnect.Entities.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public string LearnerId { get; set; }

        public int CourseId { get; set; }

        public int SessionsPurchased { get; set; }

        public int SessionsRemaining { get; set; }

        public string Status { get; set; } = null!;

        //public DateTime PurchaseDate { get; set; }

        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; } = "None";

        public string? UpdateBy { get; set; }

        public virtual Course Course { get; set; } = null!;

        public virtual ApplicationUser Learner { get; set; } = null!;

        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
