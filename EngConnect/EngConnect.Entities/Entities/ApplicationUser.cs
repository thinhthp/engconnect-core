using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Entities.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? CreateBy { get; set; } = "None";
        public string? UpdateBy { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        public virtual TutorProfile? TutorProfile { get; set; }
    }
}
